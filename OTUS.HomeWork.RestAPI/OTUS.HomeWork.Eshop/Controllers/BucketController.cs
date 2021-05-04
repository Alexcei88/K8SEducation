using Microsoft.AspNetCore.Mvc;
using OTUS.HomeWork.EShop.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OTUS.HomeWork.EShop.Domain.DTO;

namespace OTUS.HomeWork.EShop.Controllers
{
    [ApiController]
    [Route("api/bucket")]
    public class BucketController : Controller
    {
        [HttpPut("{userId}")]
        public async Task<ActionResult> UpdateBucket([FromRoute] Guid userId, BucketDTO bucket)
        {
            return null;
        }
    }
}
