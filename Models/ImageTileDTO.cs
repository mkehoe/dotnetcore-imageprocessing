using System;
using Microsoft.AspNetCore.Http;

namespace ImageProcessingWebApi.Models
{
    public class ImageTileDTO
    {
        public IFormFile Image { get; set; }

    }
}
