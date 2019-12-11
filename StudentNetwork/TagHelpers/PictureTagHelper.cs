using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using StudentNetwork.Models;

namespace StudentNetwork.TagHelpers
{
    public class PictureTagHelper : TagHelper
    {
        public Image Image{get;set;}
        public bool Round { get; set; }
        public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (Image != null && Image.Bytes != null && output != null)
            {
                output.TagName = "img";
                output.TagMode = TagMode.SelfClosing;
                output.Attributes.SetAttribute("src", $"data:image/jpeg;base64,{Convert.ToBase64String(Image.Bytes)}");
                if (Round)
                    output.Attributes.SetAttribute("style", "width: 100px;height: 100px;border-radius: 50%;");
            }
            return base.ProcessAsync(context, output);
        }
    }
}
