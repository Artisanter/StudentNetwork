using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using StudentNetwork.Models;

namespace StudentNetwork.TagHelpers
{
    public class ImageTagHelper : TagHelper
    {
        public Image Image{get;set;}

        public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if(Image != null && Image.Bytes != null)
                output.Attributes.SetAttribute("src", $"data:image/jpeg;base64,{Convert.ToBase64String(Image.Bytes)}");
            return base.ProcessAsync(context, output);
        }
    }
}
