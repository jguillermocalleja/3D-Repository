//  Copyright 2011 U.S. Department of Defense

//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at

//      http://www.apache.org/licenses/LICENSE-2.0

//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.



using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
/// <summary>
/// 
/// </summary>
public partial class Controls_MissingTextures : Website.Pages.ControlBase
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Render(object sender, EventArgs e)
    {
        oldFileName.Text = OldFile;
    }
    /// <summary>
    /// 
    /// </summary>
    public Stream FileContent { get { return FileUpload1.FileContent; } }
    /// <summary>
    /// 
    /// </summary>
    public String FileName { get { return FileUpload1.FileName; } }
    /// <summary>
    /// 
    /// </summary>
    public String OldFile { get; set; }
}
