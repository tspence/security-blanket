using ExampleApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

namespace WebApiExperiment.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SecurityPolicyDemoController : ControllerBase
    {
        private readonly ILogger<SecurityPolicyDemoController> _logger;

        public SecurityPolicyDemoController(ILogger<SecurityPolicyDemoController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// This API returns non-sensitive data that is available to everyone.
        /// Regardless of who you are, this API will always succeed.
        /// </summary>
        /// <returns></returns>
        [HttpGet("PublicData")]
        public IEnumerable<PublicDataModel> GetPublicData()
        {
            return new PublicDataModel[] { new PublicDataModel() };
        }

        /// <summary>
        /// This API demonstrates how SecurityBlanket will determine whether
        /// or not you are permitted to fetch data. A safe program will have two
        /// layers of protection, but this API demonstrates how SecurityBlanket
        /// can save you if there's a bug in one of your layers.
        /// 
        /// To see this code work as intended, call this API with IdToFetch == MyAccountId.
        /// To see SecurityBlanket catch the error in the code, call it with the two ID
        /// numbers mismatched.
        /// </summary>
        /// <param name="IdToFetch">The account IDs to fetch</param>
        /// <param name="MyAccountId">The account ID of the user making the API call</param>
        /// <returns></returns>
        [HttpGet("PrivateData")]
        public IEnumerable<PrivateDataModel> GetPrivateData(int IdToFetch, int MyAccountId)
        {
            // For simplicity sake, we will presume this is the account ID of the user
            // contacting the API.  This is grossly oversimplified for the purpose of
            // the demo.
            HttpContext.Items.Add("AccountId", MyAccountId);

            // Now let's get to work generating some results
            List<PrivateDataModel> result = new();

            // This code has a bug. Instead of fetching private data for all IDs,
            // it should have verified that the user is permitted to see the ID
            // prior to returning the data model.  Fortunately, SecurityBlanket
            // catches this error and alerts you to fix this API.
            result.Add(new PrivateDataModel() { AccountId = IdToFetch });

            return result;
        }
    }
}