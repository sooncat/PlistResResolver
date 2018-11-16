using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CE.iPhone.PList;
using System.IO;
using System.Threading;

namespace PlistResResolver
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            InitUI();
        }

        void InitUI()
        {
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            
            FlowLayoutPanel rootPanel = new FlowLayoutPanel();
            rootPanel.FlowDirection = FlowDirection.TopDown;
            rootPanel.AutoSize = true;
            rootPanel.Dock = DockStyle.Fill;
            this.Controls.Add(rootPanel);

            //firstRow
            FlowLayoutPanel firstRowPanel = new FlowLayoutPanel();
            firstRowPanel.FlowDirection = FlowDirection.LeftToRight;
            firstRowPanel.WrapContents = true;
            firstRowPanel.AutoSize = true;
            firstRowPanel.Dock = DockStyle.Fill;
            rootPanel.Controls.Add(firstRowPanel);

            Label plistDirLable = new Label();
            plistDirLable.Text = "请选择Plist文件及资源所在目录";//"PListFileDir";
            plistDirLable.AutoSize = true;
            plistDirLable.Anchor = AnchorStyles.None;
            firstRowPanel.Controls.Add(plistDirLable);

            plistDirTextBox = new TextBox();
            plistDirTextBox.Anchor = AnchorStyles.None;
            plistDirTextBox.AutoSize = true;
            firstRowPanel.Controls.Add(plistDirTextBox);

            plistDirButton = new Button();
            plistDirButton.Anchor = AnchorStyles.None;
            plistDirButton.Text = "Select...";
            plistDirButton.AutoSize = true;
            plistDirButton.Click += PlistDirButton_Click;
            firstRowPanel.Controls.Add(plistDirButton);

            //checkbox
            hasAlphaSourceCheckbox = new CheckBox();
            hasAlphaSourceCheckbox.Text = "是否有单独的Alpha图片";
            hasAlphaSourceCheckbox.CheckedChanged += OnAlphaSourceCheckBoxChanged;
            hasAlphaSourceCheckbox.AutoSize = true;
            hasAlphaSourceCheckbox.Margin = new Padding(20, 0, 0, 0);
            rootPanel.Controls.Add(hasAlphaSourceCheckbox);

            //second Row
            FlowLayoutPanel secondRowPanel = new FlowLayoutPanel();
            secondRowPanel.FlowDirection = FlowDirection.LeftToRight;
            secondRowPanel.WrapContents = false;
            secondRowPanel.AutoSize = true;
            secondRowPanel.Dock = DockStyle.Fill;
            rootPanel.Controls.Add(secondRowPanel);

            Label exportDirLable = new Label();
            exportDirLable.Text = "请选择导出目录";//"ExportFileDir";
            exportDirLable.AutoSize = true;
            exportDirLable.Anchor = AnchorStyles.None;
            secondRowPanel.Controls.Add(exportDirLable);

            exportDirTextBox = new TextBox();
            exportDirTextBox.AutoSize = true;
            exportDirTextBox.Dock = DockStyle.Fill;
            secondRowPanel.Controls.Add(exportDirTextBox);

            exportDirButton = new Button();
            exportDirButton.Text = "Select...";
            exportDirButton.AutoSize = true;
            exportDirButton.Click += ExportDirButton_Click;
            secondRowPanel.Controls.Add(exportDirButton);

            //checkbox
            drawPolygonCheckbox = new CheckBox();
            drawPolygonCheckbox.Text = "导出时是否绘制多边形（一般调试用）";//"isDrawPolygon";
            drawPolygonCheckbox.CheckedChanged += OnDrawPolygonCheckBoxChanged;
            drawPolygonCheckbox.AutoSize = true;
            drawPolygonCheckbox.Margin = new Padding(20, 0, 0, 0);
            rootPanel.Controls.Add(drawPolygonCheckbox);

            //last Row
            //second Row
            FlowLayoutPanel lastRowPanel = new FlowLayoutPanel();
            lastRowPanel.FlowDirection = FlowDirection.RightToLeft;
            lastRowPanel.AutoSize = true;
            lastRowPanel.Dock = DockStyle.Fill;
            rootPanel.Controls.Add(lastRowPanel);

            exportButton = new Button();
            exportButton.Text = "Export";
            exportButton.AutoSize = true;
            exportButton.Click += OnExportClick;
            lastRowPanel.Controls.Add(exportButton);

            progressBar = new ProgressBar();
            progressBar.AutoSize = true;
            progressBar.Value = 0;
            progressBar.Visible = false;
            progressBar.Dock = DockStyle.Fill;
            rootPanel.Controls.Add(progressBar);

            tips = new Label();
            tips.AutoSize = true;
            tips.Dock = DockStyle.Fill;
            rootPanel.Controls.Add(tips);

            InitializeComponent();
            //获取UI线程同步上下文
            _syncContext = SynchronizationContext.Current;
        }

        private void ExportDirButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "请选择导出路径";
            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.Cancel)
            {
                return;
            }
            string folderPath = dialog.SelectedPath.Trim();
            DirectoryInfo theFolder = new DirectoryInfo(folderPath);
            if (theFolder.Exists)
            {
                exportDirTextBox.Text = theFolder.FullName;
            }
        }

        private void PlistDirButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();

            dialog.Description = "请选择文件路径";
            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.Cancel)
            {
                return;
            }
            string folderPath = dialog.SelectedPath.Trim();
            DirectoryInfo theFolder = new DirectoryInfo(folderPath);
            if (theFolder.Exists)
            {
                plistDirTextBox.Text = theFolder.FullName;
            }
        }

        void OnExportClick(object sender, EventArgs e)
        {
            string plistDirPath = plistDirTextBox.Text;
            if (!Directory.Exists(plistDirPath))
            {
                MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);
                MessageBox.Show(this, "Plist Path Not Exist : " + plistDirPath, "Error");
                return;
            }
            string exportDirPath = exportDirTextBox.Text;
            if (!Directory.Exists(exportDirPath))
            {
                MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);
                MessageBox.Show(this, "Export Path Not Exist : " + plistDirPath, "Error");
                return;
            }

            Thread fThread = new Thread(new ThreadStart(Export));//开辟一个新的线程
            fThread.Start();

            EnableUI(false);
        }

        void EnableUI(bool isUsable)
        {
            plistDirTextBox.Enabled = isUsable;
            exportDirTextBox.Enabled = isUsable;
            progressBar.Visible = !isUsable;


            exportButton.Enabled = isUsable;
            drawPolygonCheckbox.Enabled = isUsable;
            hasAlphaSourceCheckbox.Enabled = isUsable;
            plistDirButton.Enabled = isUsable;
            exportDirButton.Enabled = isUsable;
        }

        void Export()
        {
            string exportDirPath = exportDirTextBox.Text;
            string plistDirPath = plistDirTextBox.Text;
            string[] plistFiles = Directory.GetFiles(plistDirPath, "*.plist");

            //progressBar.Maximum = plistFiles.Length;
            _syncContext.Post(SetProgress, plistFiles.Length);

            for (int i=0; i < plistFiles.Length;i++)
            {
                string pFile = plistFiles[i];
                //Console.Out.WriteLine("pfile = " + pFile);
                ExportPlist(pFile, exportDirPath);
                //progressBar.Value = i;
                _syncContext.Post(UpdateProgress, i+1);
            }

            _syncContext.Post(ShowMessage, "Finished");//子线程中通过UI线程上下文更新UI

        }

        void SetProgress(object state)
        {
            progressBar.Maximum = (int)state;
        }

        void UpdateProgress(object state)
        {
            progressBar.Value = (int)state;
        }

        private void ShowMessage(object state)
        {
            MessageBoxHelper.PrepToCenterMessageBoxOnForm(this);
            MessageBox.Show((string)state, "Message");
            //tips.Text = "state";
            EnableUI(true);
        }

        void OnAlphaSourceCheckBoxChanged(object sender, EventArgs e)
        {
            hasAlphaSource = ((CheckBox)sender).Checked;
        }

        void OnDrawPolygonCheckBoxChanged(object sender, EventArgs e)
        {
            isDrawPolygon = ((CheckBox)sender).Checked;
        }

        //private void button1_Click(object sender, EventArgs e)
        //{
        //    string path = @"E:\MyWorkSpace\CS\PlistResResolver\PlistResResolver\testres\battleStart.plist";
        //    string destPath = @"E:\MyWorkSpace\CS\PlistResResolver\PlistResResolver\testres";
        //    ExportPlist(path, destPath);
        //}

        void ExportPlist(string plistFilePath, string exportDestPath)
        {
            PListRoot root = PListRoot.Load(plistFilePath);
            PListDict dic = (PListDict)root.Root;
            PListDict frames = (PListDict)dic["frames"];
            PListDict metadata = (PListDict)dic["metadata"];

            //
            string texFileName = (PListString)metadata["textureFileName"];
            texFileName = texFileName.Replace(".ccz", ".png");
            string dir = Path.GetDirectoryName(plistFilePath);
            string texPath = Path.Combine(dir, texFileName);
            string alphaTexPath = texPath.Insert(texPath.IndexOf("."), "@alpha");

            Console.Out.WriteLine("texPath = " + texPath + "alphaTex = " + alphaTexPath);

            try
            {
                if (!File.Exists(texPath))
                {
                    _syncContext.Post(ShowMessage, "File Not Found:"+texPath);
                    return;
                }
                Bitmap image = new Bitmap(texPath, false);
                Bitmap alpha = null;
                if (hasAlphaSource)
                {
                    if (!File.Exists(alphaTexPath))
                    {
                        _syncContext.Post(ShowMessage, "File Not Found:" + alphaTexPath);
                        return;
                    }
                    alpha = new Bitmap(alphaTexPath, false);
                }
                //
                foreach (KeyValuePair<string, IPListElement> pair in frames)
                {
                    string picName = pair.Key;
                    string newPicPath = Path.Combine(exportDestPath, picName);
                    //Console.Out.WriteLine("newPicPath = " + newPicPath);
                    ReadPic(newPicPath, (PListDict)pair.Value, image, alpha);
                }
            }
            catch (Exception e)
            {
                _syncContext.Post(ShowMessage, e.ToString());
            }
            Console.Out.WriteLine("End");
        }

        void ReadPic(string newPicPath, PListDict frames, Bitmap tex, Bitmap alphaTex)
        {
            Vector spriteOffset = ParseVec((PListString)frames["spriteOffset"]);          //包络框中心相对于图片中心的偏移
            Vector spriteSize = ParseVec((PListString)frames["spriteSize"]);              //包络框
            Vector spriteSourceSize = ParseVec((PListString)frames["spriteSourceSize"]);  //原始图片尺寸
            Rect textureRect = ParseRect((PListString)frames["textureRect"]);               //包络框在拼合图片中的位置
            bool textureRotated = (PListBool)frames["textureRotated"];                      //旋转
            int[] triangles = ParseIntArray((PListString)frames["triangles"]);              //三角形顶点顺序
            int[] vertices = ParseIntArray((PListString)frames["vertices"]);                //三角形顶点坐标（相对于原始图片尺寸）
            int[] verticesUV = ParseIntArray((PListString)frames["verticesUV"]);            //三角形顶点UV（相对于拼合图片尺寸）

            CreatePic(newPicPath, spriteOffset, spriteSize, spriteSourceSize, textureRect, triangles, vertices, tex, alphaTex);
        }

        void CreatePic(string newPicPath, 
            Vector spriteOffset, Vector spriteSize, Vector spriteSourceSize, 
            Rect textureRect, int[] triangles, int[] vertices,
            Bitmap tex, Bitmap alphaTex)
        {
            List<Triangle> allTriangles = ParseTriangle(triangles, vertices);
            //Console.Out.WriteLine("triangle.count = " + allTriangles.Count);
            //List<Triangle> combinedTriangles = ParseTriangle(triangles, verticesUV);
            
            Bitmap newPic = new Bitmap((int)spriteSourceSize.x, (int)spriteSourceSize.y);

            for (int x = 0; x < spriteSize.x; x++)
            {
                for (int y = 0; y < spriteSize.y; y++)
                {
                    Vector relativeP = new Vector(x, y);    //这是包络框下的坐标
                    Vector destP = GetDestP(relativeP, spriteOffset, spriteSize, spriteSourceSize);//这是原图的坐标。
                    for (int k = 0; k < allTriangles.Count;k++)
                    {
                        if (allTriangles[k].ContainsPoint(destP))
                        {
                            Vector sourceP = new Vector(textureRect.x + x, textureRect.y + y);//这是组合图上的坐标
                            CopyPixel(sourceP, destP, tex, newPic, alphaTex);
                            break;
                        }
                    }
                }
            }

            if (isDrawPolygon)
            {
                DrawTriangles(newPic, allTriangles);
            }

            string destDirPath = Path.GetDirectoryName(newPicPath);
            if (!Directory.Exists(destDirPath))
            {
                Directory.CreateDirectory(destDirPath);
            }
            newPic.Save(newPicPath);
            //Console.Out.WriteLine("Save Pic " + newPicPath);
            _syncContext.Post(UpdateTips, newPicPath);
        }

        void UpdateTips(object state)
        {
            tips.Text = (string)state;
        }

        void DrawTriangles(Bitmap pic, List<Triangle> triangles)
        {
            Pen blackPen = new Pen(Color.Red, 1);
            Graphics graphics = Graphics.FromImage(pic);
            foreach (Triangle t in triangles)
            {
                graphics.DrawLine(blackPen, t.a.x, t.a.y, t.b.x, t.b.y);
                graphics.DrawLine(blackPen, t.a.x, t.a.y, t.c.x, t.c.y);
                graphics.DrawLine(blackPen, t.b.x, t.b.y, t.c.x, t.c.y);
            }
        }

        Vector GetDestP(Vector relativeP, Vector spriteOffset, Vector spriteSize, Vector spriteSourceSize)
        {
            float relativeX = relativeP.x;
            float diffX = spriteSourceSize.x / 2 + spriteOffset.x - spriteSize.x / 2;
            float destX = diffX + relativeX;

            float relativeY = relativeP.y;
            float diffY = spriteSourceSize.y / 2 - spriteOffset.y - spriteSize.y / 2;
            float destY = diffY + relativeY;

            return new Vector(destX, destY);
        }

        void CopyPixel(Vector sourceP, Vector destP, Bitmap source, Bitmap dest, Bitmap alphaSource)
        {
            //Console.Out.WriteLine("copy form " + sourceP + ", to " + destP);
            Color c = source.GetPixel((int)sourceP.x, (int)sourceP.y);
            if (hasAlphaSource)
            {
                Color alpha = alphaSource.GetPixel((int)sourceP.x, (int)sourceP.y);
                c = Color.FromArgb(alpha.R, c.R, c.G, c.B);
            }
            dest.SetPixel((int)destP.x, (int)destP.y, c);
        }

        List<Triangle> ParseTriangle(int[] triangles, int[] vertices)
        {
            List<Triangle> allTriangles = new List<Triangle>();
            for (int i = 0; i < triangles.Length; i += 3)
            {
                int aIndex = triangles[i];
                int bIndex = triangles[i + 1];
                int cIndex = triangles[i + 2];

                Vector a = new Vector(vertices[aIndex * 2], vertices[aIndex * 2 + 1]);
                Vector b = new Vector(vertices[bIndex * 2], vertices[bIndex * 2 + 1]);
                Vector c = new Vector(vertices[cIndex * 2], vertices[cIndex * 2 + 1]);

                allTriangles.Add(new Triangle(a, b, c));
            }

            return allTriangles;
        }


        Vector ParseVec(string str)
        {
            string middles = str.Substring(1, str.Length - 2);
            string[] singles = middles.Split(new char[] { ',' });
            float x = float.Parse(singles[0]);
            float y = float.Parse(singles[1]);
            return new Vector(x, y);
        }

        Rect ParseRect(string str)
        {
            string rect = str.Replace("{", "").Replace("}", "");
            string[] divides = rect.Split(',');
            float[] result = new float[divides.Length];
            for (int i = 0; i < divides.Length; i++)
            {
                result[i] = float.Parse(divides[i]);
            }
            return new Rect(result[0], result[1], result[2], result[3]);
        }

        int[] ParseIntArray(string str)
        {
            string[] datas = str.Split(new char[] { ' ' });
            int[] result = new int[datas.Length];
            for (int i = 0; i < datas.Length; i++)
            {
                result[i] = int.Parse(datas[i]);
            }
            return result;
        }

        ////////
        
        /// <summary>
        /// 是否带单独的alpha图片
        /// </summary>
        bool hasAlphaSource;

        /// <summary>
        /// 是否在导出图片中绘制多边形（debug用）
        /// </summary>
        bool isDrawPolygon;

        TextBox exportDirTextBox;
        TextBox plistDirTextBox;
        ProgressBar progressBar;
        Button exportButton;
        CheckBox drawPolygonCheckbox;
        CheckBox hasAlphaSourceCheckbox;
        Button plistDirButton;
        Button exportDirButton;
        Label tips;

        SynchronizationContext _syncContext = null;
    }
}
