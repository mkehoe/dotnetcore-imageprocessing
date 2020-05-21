using System;
using System.Collections.Generic;
using ImageProcessingWebApi.Models;

namespace ImageProcessingWebApi.Stores
{
    public class TimestampsStore
    {
        private readonly IList<TimestampDTO> _resizeImageTimestamp;
        private readonly IList<TimestampDTO> _tileImageTimestamp;

        public TimestampsStore()
        {
            _resizeImageTimestamp = new List<TimestampDTO>();
            _tileImageTimestamp = new List<TimestampDTO>();
        }
            
        public void AddTileTimestamps(TimestampDTO timestamps)
        {
            _tileImageTimestamp.Add(timestamps);
        }

        public void AddResizeTimestamps(TimestampDTO timestamps)
        {
            _resizeImageTimestamp.Add(timestamps);
        }

        public IList<TimestampDTO> GetTileTimestamps()
        {
            return _tileImageTimestamp;
        }

        public IList<TimestampDTO> GetResizeTimestamps()
        {
            return _resizeImageTimestamp;
        }

        public void Reset()
        {
            _resizeImageTimestamp.Clear();
            _tileImageTimestamp.Clear();
        }

    }
}
