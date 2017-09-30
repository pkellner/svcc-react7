using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.NodeServices;
using System.IO;
using Microsoft.AspNetCore.StaticFiles;

namespace WebApp.Controllers
{
    public class ResizeImageController : Controller
    {

        private const int MaxDimension = 5000;
        private static string[] AllowedMimeTypes = new[] { "image/jpeg", "image/png", "image/gif" };

        //private IHostingEnvironment _environment;
        //private INodeServices _nodeServices;

     


        //public ResizeImageController(IHostingEnvironment environment, INodeServices nodeServices)
        //{
        //    _environment = environment;
        //    _nodeServices = nodeServices;
        //}

        public IActionResult Index()
        {
            return View();
        }

        [Route("resize/{*imagePath}")]
        public async Task<IActionResult> Resize(
            [FromServices] INodeServices nodeServices,
            [FromServices] IHostingEnvironment environment,
            string imagePath, int maxWidth, int maxHeight)
        {
            // Validate incoming params		
            if (maxWidth < 0 || maxHeight < 0 || maxWidth > MaxDimension || maxHeight > MaxDimension
                 || (maxWidth + maxHeight) == 0)
            {
                //return BadRequest("Invalid dimensions");
                maxWidth = 100;
                maxHeight = 100;
            }

            var mimeType = GetContentType(imagePath);
            if (Array.IndexOf(AllowedMimeTypes, mimeType) < 0)
            {
                return BadRequest("Disallowed image format");
            }

            // Locate source image on disk		
            var fileInfo = environment.WebRootFileProvider.GetFileInfo(imagePath);
            if (!fileInfo.Exists)
            {
                return NotFound();
            }

            // Invoke Node and pipe the result to the response
            var imageStream = await nodeServices.InvokeAsync<Stream>(
                "./NodeSrc/resizeimage",
                fileInfo.PhysicalPath,
                mimeType,
                maxWidth,
                maxHeight);
            return File(imageStream, mimeType);
        }

        private string GetContentType(string path)
        {
            string result;
            return new FileExtensionContentTypeProvider().TryGetContentType(path, out result) ? result : null;
        }
    }



    //var stream = await nodeServices.InvokeAsync<Stream>(
    //    "imageresizer.js", fileInfo.PhysicalPath, maxWidth);

    //var contentType = "image/jpeg";
    //return new FileStreamResult(stream, contentType);
}
