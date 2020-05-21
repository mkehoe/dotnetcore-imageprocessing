using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ImageProcessingWebApi.Models;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using ImageProcessingWebApi.Stores;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;

namespace ImageProcessingWebApi.Controllers
{
    [ApiController]
    [Route("images")]
    public class ImageProcessingController : ControllerBase
    {
        private readonly ILogger<ImageProcessingController> _logger;
        private readonly TimestampsStore _timestamps;


        public ImageProcessingController(ILogger<ImageProcessingController> logger, TimestampsStore timestamps)
        {
            _logger = logger;
            _timestamps = timestamps;
        }
        
        [HttpGet("stats")]
        public IActionResult GetStats()
        {
            return Ok(new { resizeTimestamps = _timestamps.GetResizeTimestamps(), tileTimestamps = _timestamps.GetTileTimestamps() });
        }

        [HttpPost("reset_stats")]
        public IActionResult ResetStats()
        {
            _timestamps.Reset();
            return Ok();
        }

        [HttpPost("resize")]
        public async Task<IActionResult> ResizeImage([FromForm]ImageResizeDTO imageResizeDTO)
        {
            try
            {
                var sw = new Stopwatch();
                sw.Start();

                var timestamps = new TimestampDTO();

                if (imageResizeDTO.Image.Length <= 0)
                {
                    return BadRequest();
                }

                using var memStream = new MemoryStream();

                await imageResizeDTO.Image.CopyToAsync(memStream);

                timestamps.t1 = sw.ElapsedMilliseconds;

                using var image = new Bitmap(memStream);

                timestamps.t2 = sw.ElapsedMilliseconds;

                var resized = new Bitmap(imageResizeDTO.ResizeWidth, imageResizeDTO.ResizeHeight);
                using var graphics = Graphics.FromImage(resized);

                timestamps.t3 = sw.ElapsedMilliseconds;

                graphics.CompositingQuality = CompositingQuality.HighSpeed;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.DrawImage(image, 0, 0, imageResizeDTO.ResizeWidth, imageResizeDTO.ResizeHeight);

                timestamps.t4 = sw.ElapsedMilliseconds;

                using var outStream = new MemoryStream();
                resized.Save(outStream, ImageFormat.Jpeg);

                sw.Stop();

                timestamps.t5 = sw.ElapsedMilliseconds;

                _timestamps.AddResizeTimestamps(timestamps);

                return new FileContentResult(outStream.ToArray(), "image/jpeg");
            }catch(Exception ex)
            {
                Console.WriteLine($"{ex}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("tile")]
        public async Task<IActionResult> TileImage([FromForm]ImageTileDTO imageTileDTO)
        {
            try
            {
                var sw = new Stopwatch();
                sw.Start();
                
                var timestamps = new TimestampDTO();
                
                if (imageTileDTO.Image.Length <= 0)
                {
                    return BadRequest();
                }

                using var memStream = new MemoryStream();

                await imageTileDTO.Image.CopyToAsync(memStream);

                timestamps.t1 = sw.ElapsedMilliseconds;

                using var image = new Bitmap(memStream);

                timestamps.t2 = sw.ElapsedMilliseconds;

                int halfWidth = image.Width / 2;
                int halfHeight = image.Height / 2;
                var resized = new Bitmap(halfWidth, halfHeight);
                using var graphics = Graphics.FromImage(resized);

                timestamps.t3 = sw.ElapsedMilliseconds;

                graphics.CompositingQuality = CompositingQuality.HighSpeed;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.DrawImage(image, 0, 0, halfWidth, halfHeight);
                graphics.DrawImage(image, halfWidth, halfHeight, image.Width, image.Height);
                graphics.DrawImage(image, 0, halfHeight, halfWidth, image.Height);
                graphics.DrawImage(image, halfWidth, 0, image.Width, halfHeight);

                timestamps.t4 = sw.ElapsedMilliseconds;

                using var outStream = new MemoryStream();
                resized.Save(outStream, ImageFormat.Jpeg);

                timestamps.t5 = sw.ElapsedMilliseconds;
                sw.Stop();

                _timestamps.AddTileTimestamps(timestamps);

                return new FileContentResult(outStream.ToArray(), "image/jpeg");
            }
            catch(Exception ex)
            {
                Console.WriteLine($"{ex}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

    }
}
