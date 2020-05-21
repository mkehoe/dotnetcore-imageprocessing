using System;
using Microsoft.AspNetCore.Http;

namespace ImageProcessingWebApi.Models
{
    public class ImageResizeDTO
    {        
        public IFormFile Image { get; set; }

        public int ResizeWidth { get; set; }
        public int ResizeHeight { get; set; }
    }
}
