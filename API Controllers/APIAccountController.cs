using Microsoft.AspNetCore.Mvc;
using Social_Media.Models;
using Neo4j.Driver;
using System.Text.RegularExpressions;

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
                if (!IsValidPassword(newUser.Password))
                {
                    return BadRequest("Invalid password format. Please follow the password requirements.");
                }

                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(newUser.Password);

                using (var session = _driver.AsyncSession(o => o.WithDatabase("social")))
                {
                    await session.WriteTransactionAsync(async tx =>
                    {
                        var createUserQuery = $@"
                    CREATE (u:User {{username: '{newUser.Username}', email: '{newUser.Email}', password: '{hashedPassword}'}})
                ";
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


        private bool IsValidPassword(string password)
        {
            var regexPattern = @"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$";
            var regex = new Regex(regexPattern);

            return regex.IsMatch(password);
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
                        var checkLoginQuery = "MATCH (u:User {username: $username, password: $password}) RETURN COUNT(u) as userCount";
                        var parameters = new { username = loginModel.Username, password = loginModel.Password };

                        var cursor = await tx.RunAsync(checkLoginQuery, parameters);
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
                        ModelState.AddModelError("", "Invalid username or password.");
                        return Unauthorized("Invalid username or password.");
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Login failed: {ex.Message}");
                return BadRequest($"Login failed: {ex.Message}");
            }
        }
    }
}
