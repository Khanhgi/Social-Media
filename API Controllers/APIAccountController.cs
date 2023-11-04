using Microsoft.AspNetCore.Mvc;
using Social_Media.Models;
using Neo4j.Driver;

namespace Social_Media.API_Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class APIAccountController : Controller
    {
        private readonly IDriver _driver;

        public APIAccountController(IDriver driver)
        {
            _driver = driver;
        }

        [HttpPost("Register")]
        [Obsolete("This method is deprecated, use NewMethod instead.")]
        public async Task<IActionResult> Register([FromBody] User newUser)
        {
            try
            {
                using (var session = _driver.AsyncSession(o => o.WithDatabase("social")))
                {
                    await session.WriteTransactionAsync(async tx =>
                    {
                        var createUserQuery = $@"
                        CREATE (u:User {{
                            username: '{newUser.Username}',
                            email: '{newUser.Email}',
                            password: '{newUser.Password}'
                        }})";
                        await tx.RunAsync(createUserQuery);
                    });
                }

                return Ok("Registration successful");
            }
            catch (Exception ex)
            {
                return BadRequest($"Registration failed: {ex.Message}");
            }
        }

        [HttpPost("Login")]
        [Obsolete("This method is deprecated, use NewMethod instead.")]
        public async Task<IActionResult> Login([FromBody] Login loginModel)
        {
            try
            {
                using (var session = _driver.AsyncSession(o => o.WithDatabase("social")))
                {
                    var result = await session.ReadTransactionAsync(async tx =>
                    {
                        var checkLoginQuery = $@"
                        MATCH (u:User {{username: '{loginModel.Username}', password: '{loginModel.Password}'}})
                        RETURN count(u) as userCount";

                        var cursor = await tx.RunAsync(checkLoginQuery);
                        var record = await cursor.SingleAsync();

                        var userCount = record["userCount"].As<int>();
                        return userCount > 0;
                    });

                    if (result)
                    {
                        return Ok("Login successful");
                    }
                    else
                    {
                        return Unauthorized("Invalid login attempt");
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Login failed: {ex.Message}");
            }
        }
    }
}
