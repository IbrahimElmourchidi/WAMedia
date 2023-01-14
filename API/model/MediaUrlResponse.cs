using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.model;

public class MediaUrlResponse
{

    public string url { get; set; }
    public string mime_type { get; set; }
    public string sha256 { get; set; }
    public int file_size { get; set; }
    public string id { get; set; }
    public string messaging_product { get; set; }

}
