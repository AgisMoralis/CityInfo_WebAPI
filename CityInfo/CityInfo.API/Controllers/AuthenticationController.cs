using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CityInfo.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiVersion(0.1)]
    [ApiVersion(1)]
    [ApiVersion(2)]
    public class AuthenticationController : Controller
    {
        /// <summary>
        /// The credentials provided by the user during the try to establish a secure connection
        /// </summary>
        public class AuthenticationRequestBody
        {
            /// <summary>
            /// The username of the user credentials
            /// </summary>
            public string? Username { get; set; }

            /// <summary>
            /// The password of the user credentials
            /// </summary>
            public string? Password { get; set; }
        }

        public class CityInfoUser
        {
            // Public properties
            public int UserID { get; set; }
            public string Username { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string City { get; set; }

            public CityInfoUser(int userID, string username, string firstName, string lastName, string city)
            {
                UserID = userID;
                Username = username;
                FirstName = firstName;
                LastName = lastName;
                City = city;
            }
        }

        // Private members
        private readonly ILogger<AuthenticationController> _logger;
        private readonly IConfiguration _configuration;

        public AuthenticationController(ILogger<AuthenticationController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        /// <summary>
        /// Verifies the input credentials defined by the user and returns a JWT token to secure the connection with the API
        /// </summary>
        /// <param name="authenticationRequestBody">The credentials (username and password) indicated by the user during the connection try</param>
        /// <returns>A JWT token to secure the connection with the API</returns>
        /// <response code="200">Returns the JWT token for secure connection</response>
        [HttpPost("authenticate")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<string> Authenticate(AuthenticationRequestBody authenticationRequestBody)
        {
            try
            {
                // Step 1: Validate the username and the password
                var user = ValidateUserCredentials(authenticationRequestBody.Username, authenticationRequestBody.Password);
                if (user == null)
                {
                    return Unauthorized();
                }

                // Step 2: Create a token

                // "Header": The security key is used to generate the signing credentials that will
                // be used later to generate the token. The "Header" of the generated token includes
                // those information about what algorithm was used (here the HS256) and the type of it.
                var securityKey = new SymmetricSecurityKey(Convert.FromBase64String(_configuration["Authentication:SecretForKey"]));
                var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                // "Payload": The claims are part of the "Payload" of the token that will be generated
                // and can include any information we want (sub, given_name, family_name are
                // conventions that are widely used in the claims, that's why we use them here).
                var claimsForToken = new List<Claim>();
                claimsForToken.Add(new Claim("sub", user.UserID.ToString()));
                claimsForToken.Add(new Claim("given_name", user.FirstName));
                claimsForToken.Add(new Claim("family_name", user.LastName));
                claimsForToken.Add(new Claim("city", user.City));

                // Here the toke is generated. The last part of the token, the "Signature"
                // is added in the end of the token.
                var jwtSecurityToken = new JwtSecurityToken(
                    _configuration["Authentication:Issuer"],
                    _configuration["Authentication:Audience"],
                    claimsForToken,
                    DateTime.UtcNow,
                    DateTime.UtcNow.AddHours(1),
                    signingCredentials);
                var tokenToReturn = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

                // IMPORTANT INFO: The tokens are not encrypted but they are only encoded.
                // That means token-based security relies on HTTPS protocol for encryption.
                // This is why, when tokens are used, HTTPS is a must-use.
                return Ok(tokenToReturn);
            }
            catch (Exception ex)
            {
                _logger.LogCritical("Unexpected exception occurred when trying to verify the credentials.", ex);
                return StatusCode(500, $"A problem occurred while handling your request.");
            }
        }

        private CityInfoUser ValidateUserCredentials(string? username, string? password)
        {
            // Note: In this project we don't have a table in the "CityInfo" DB or
            // a separate DB that store the credentials of the users, so that we can
            // validate the passed-through username and password values from the HTTP
            // POST request against to what is stored inside that DB. Therefore here
            // we assume that the passed credentials from any requests are valid.

            string GetUserCity(string usernameStr) => usernameStr switch
            {
                var s when !string.IsNullOrWhiteSpace(s) && s.Contains("NYC") => "NYC",
                var s when !string.IsNullOrWhiteSpace(s) && s.Contains("Athens") => "Athens",
                var s when !string.IsNullOrWhiteSpace(s) && s.Contains("Paris") => "Paris",
                _ => "Unknown"
            };

            return new CityInfoUser(1, "Agleoras", "Agis", "Surname", GetUserCity(username));
        }
    }
}
