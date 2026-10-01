using System;
using System.IO;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32.SafeHandles;

class DiskTest : Form
{
    const long Total = 1024L * 1024 * 1024;
    const int Block = 1024 * 1024;
    ComboBox drives = new ComboBox();
    MaterialButton start = new MaterialButton(), stop = new MaterialButton(), refresh = new MaterialButton();
    MaterialProgress bar = new MaterialProgress();
    Label state = new Label(), results = new Label();
    Label writeResult = new Label(), readResult = new Label();
    static readonly Color Accent = Color.FromArgb(255,79,65);
    CancellationTokenSource cancellation;
    bool running;
Image beast;
    [DllImport("kernel32.dll", CharSet=CharSet.Unicode, SetLastError=true)]
    static extern SafeFileHandle CreateFile(string path, uint access, uint share, IntPtr security, uint creation, uint flags, IntPtr template);
    [DllImport("kernel32.dll", SetLastError=true)]
    static extern bool WriteFile(SafeFileHandle handle, IntPtr buffer, int length, out int count, IntPtr overlapped);
    [DllImport("kernel32.dll", SetLastError=true)]
    static extern bool ReadFile(SafeFileHandle handle, IntPtr buffer, int length, out int count, IntPtr overlapped);
    [DllImport("kernel32.dll", SetLastError=true)]
    static extern IntPtr VirtualAlloc(IntPtr address, UIntPtr size, uint type, uint protect);
    [DllImport("kernel32.dll")]
    static extern bool VirtualFree(IntPtr address, UIntPtr size, uint type);
    class Target
    {
        public DriveInfo Drive;
        public override string ToString() { return Drive.Name + "  " + Drive.VolumeLabel + "  |  " + (Drive.DriveType == DriveType.Removable ? "Çıkarılabilir" : "Sabit disk") + "  |  Boş: " + (Drive.AvailableFreeSpace / 1073741824.0).ToString("F1") + " GB"; }
    }
    DiskTest()
    {
        Text="HyperDrive · Beast Core"; ClientSize=new Size(1120,760);
        AutoScaleMode=AutoScaleMode.Dpi; StartPosition=FormStartPosition.CenterScreen;
        Font=new Font("Bahnschrift",11); BackColor=Color.FromArgb(8,12,20); ForeColor=Color.FromArgb(240,246,247);
        DoubleBuffered=true; FormBorderStyle=FormBorderStyle.FixedSingle; MaximizeBox=false;
        using(var stream=typeof(DiskTest).Assembly.GetManifestResourceStream("Beast")) { if(stream!=null) using(var image=Image.FromStream(stream)) beast=new Bitmap(image); }
        AddText("H Y P E R D R I V E",52,40,550,25,12,Accent,true);
        AddText("BEAST CORE",47,73,610,106,44,ForeColor,true);
        AddText("Depolamanın içindeki canavarı uyandır.",52,185,600,30,14,Color.FromArgb(145,205,213),false);
        AddText("01 / HEDEF SÜRÜCÜ",52,224,440,24,10,Color.FromArgb(103,218,220),true);
        drives.SetBounds(52,257,459,34); drives.DropDownStyle=ComboBoxStyle.DropDownList;
        drives.FlatStyle=FlatStyle.Flat; drives.BackColor=Color.FromArgb(18,31,43); drives.ForeColor=ForeColor;
        drives.DrawMode=DrawMode.OwnerDrawFixed; drives.ItemHeight=28;
        drives.DrawItem+=delegate(object sender,DrawItemEventArgs e) { using(var b=new SolidBrush(Color.FromArgb(18,31,43))) e.Graphics.FillRectangle(b,e.Bounds); if(e.Index>=0) TextRenderer.DrawText(e.Graphics,drives.Items[e.Index].ToString(),Font,e.Bounds,ForeColor,TextFormatFlags.VerticalCenter|TextFormatFlags.EndEllipsis); };
        refresh.Text="Yenile"; refresh.SetBounds(525,252,111,44); refresh.BackColor=Color.FromArgb(25,55,63); refresh.Click+=delegate { LoadDrives(); };
        MakeCard(52,"YAZMA / WRITE",writeResult,Accent);
        MakeCard(354,"OKUMA / READ",readResult,Color.FromArgb(77,237,215));
        AddText("02 / AKTARIM NABZI",52,492,580,24,10,Color.FromArgb(103,218,220),true);
        bar.SetBounds(52,527,584,13); bar.BackColor=Color.Transparent;
        state.SetBounds(52,552,584,44); state.BackColor=Color.Transparent; state.Font=new Font("Bahnschrift",10); state.ForeColor=Color.FromArgb(179,201,212);
        start.Text="Testi başlat   →"; start.SetBounds(52,610,310,60); start.BackColor=Accent; start.ForeColor=Color.FromArgb(20,16,24); start.Click+=async delegate { await Run(); };
        stop.Text="Durdur"; stop.SetBounds(380,610,170,60); stop.BackColor=Color.FromArgb(25,55,63); stop.Enabled=false; stop.Click+=delegate { cancellation.Cancel(); stop.Enabled=false; };
        AddText("1 GB TEST   /   SIRALI OKUMA + YAZMA   /   MB/sn",52,699,720,22,9,Color.FromArgb(128,168,180),true);
        Controls.AddRange(new Control[]{drives,refresh,start,stop,bar,state});
        FormClosing+=delegate(object sender,FormClosingEventArgs e) { if(running) { cancellation.Cancel(); e.Cancel=true; state.Text="Test durduruluyor; geçici dosya temizleniyor."; } };
        LoadDrives();
    }
    void AddText(string text,int x,int y,int w,int h,int size,Color color,bool bold) {
        Controls.Add(new Label{Text=text,BackColor=Color.Transparent,ForeColor=color,Font=new Font("Bahnschrift",size,bold?FontStyle.Bold:FontStyle.Regular),TextAlign=ContentAlignment.MiddleLeft,UseCompatibleTextRendering=false,Location=new Point(x,y),Size=new Size(w,h)});
    }
    void MakeCard(int x,string title,Label value,Color color) {
        MaterialCard card=new MaterialCard(); card.SetBounds(x,327,282,142); card.BackColor=Color.FromArgb(13,25,36); card.Edge=color;
        card.Controls.Add(new Label{Text=title,BackColor=Color.Transparent,ForeColor=color,Location=new Point(24,18),Size=new Size(232,24),Font=new Font("Bahnschrift",10,FontStyle.Bold)});
        value.Text="—"; value.BackColor=Color.Transparent; value.Font=new Font("Bahnschrift",30,FontStyle.Bold); value.SetBounds(20,51,230,56); card.Controls.Add(value);
        card.Controls.Add(new Label{Text="MB/sn  ·  1 GB",BackColor=Color.Transparent,ForeColor=color,Location=new Point(25,110),Size=new Size(220,20),Font=new Font("Bahnschrift",9,FontStyle.Bold)});
        Controls.Add(card);
    }
    protected override void OnPaintBackground(PaintEventArgs e) {
        PaintBackdrop(e.Graphics,0,0);
    }
    public void PaintBackdrop(Graphics graphics,int offsetX,int offsetY) {
        var saved=graphics.Save();
        graphics.TranslateTransform(offsetX,offsetY);
        graphics.InterpolationMode=InterpolationMode.HighQualityBicubic;
        using(var background=new SolidBrush(BackColor)) graphics.FillRectangle(background,ClientRectangle);
        if(beast!=null) graphics.DrawImage(beast,ClientRectangle);
        using(var shade=new LinearGradientBrush(ClientRectangle,Color.FromArgb(155,6,13,22),Color.FromArgb(0,6,13,22),0f)) graphics.FillRectangle(shade,ClientRectangle);
        graphics.SmoothingMode=SmoothingMode.AntiAlias;
        float sx=ClientSize.Width/1120f,sy=ClientSize.Height/760f;
        using(var pen=new Pen(Color.FromArgb(120,65,235,217),1.2f*sx)) {
            graphics.DrawBezier(pen,39*sx,316*sy,8*sx,350*sy,28*sx,456*sy,44*sx,474*sy);
            graphics.DrawBezier(pen,650*sx,322*sy,685*sx,350*sy,658*sx,438*sy,650*sx,465*sy);
        }
        graphics.Restore(saved);
    }
    protected override void Dispose(bool disposing) { if(disposing && beast!=null) beast.Dispose(); base.Dispose(disposing); }
    void LoadDrives()
    {
        drives.Items.Clear();
        foreach (DriveInfo d in DriveInfo.GetDrives())
        {
            try { if(d.IsReady && (d.DriveType==DriveType.Fixed || d.DriveType==DriveType.Removable)) drives.Items.Add(new Target { Drive=d }); } catch { }
        }
        if(drives.Items.Count>0) drives.SelectedIndex=0;
        state.Text="Sürücü seçin. Mevcut dosyalar değiştirilmez; geçici test dosyası oluşturulur.";
    }
    async Task Run()
    {
        Target target=drives.SelectedItem as Target; if(target==null) return;
        if(target.Drive.AvailableFreeSpace < Total + 64L*1024*1024) { MessageBox.Show("En az 1 GB + 64 MB boş alan gerekiyor."); return; }
        cancellation=new CancellationTokenSource(); running=true;
        start.Enabled=refresh.Enabled=drives.Enabled=false; stop.Enabled=true; bar.Value=0; results.Text=""; writeResult.Text=readResult.Text="—";
        string path=null;
        var progress=new Progress<Tuple<int,string>>(p => { bar.Value=p.Item1; state.Text=p.Item2; });
        string cleanupError=null;
        try
        {
            string directory=FindTestDirectory(target.Drive.RootDirectory.FullName);
            path=Path.Combine(directory,".depolama-hiz-testi-"+Guid.NewGuid().ToString("N")+".tmp");
            double[] speeds=await Task.Run(() => Benchmark(path,cancellation.Token,progress));
            results.Text="Yazma: "+speeds[0].ToString("F1")+" MB/sn     Okuma: "+speeds[1].ToString("F1")+" MB/sn";
            writeResult.Text=speeds[0].ToString("F1"); readResult.Text=speeds[1].ToString("F1");
            state.Text="Test tamamlandı. MB/sn = saniyede 1.000.000 bayt.";
        }
        catch(OperationCanceledException) { state.Text="Test durduruldu."; }
        catch(Exception ex) { state.Text="Test başarısız."; MessageBox.Show(ex.Message,"Disk testi hatası"); }
        finally
        {
            try { if(path!=null && File.Exists(path)) File.Delete(path); } catch(Exception ex) { cleanupError=ex.Message; }
            running=false; start.Enabled=refresh.Enabled=drives.Enabled=true; stop.Enabled=false; cancellation.Dispose();
            if(cleanupError!=null) MessageBox.Show("Geçici dosya silinemedi. Elle silebilirsiniz:\n"+path+"\n"+cleanupError);
        }
    }
    static string FindTestDirectory(string root)
    {
        // A system volume's root is protected. Use a writable folder on that same volume.
        string[] candidates={Path.GetTempPath(),Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),AppDomain.CurrentDomain.BaseDirectory,root};
        foreach(string folder in candidates)
        {
            if(string.IsNullOrEmpty(folder) || !Directory.Exists(folder)) continue;
            if(!string.Equals(Path.GetPathRoot(Path.GetFullPath(folder)),root,StringComparison.OrdinalIgnoreCase)) continue;
            string probe=Path.Combine(folder,".hyperdrive-probe-"+Guid.NewGuid().ToString("N")+".tmp");
            try {
                using(var file=new FileStream(probe,FileMode.CreateNew,FileAccess.Write,FileShare.None,1,FileOptions.DeleteOnClose)) { }
                return folder;
            }
            catch(UnauthorizedAccessException) { }
            catch(IOException) { }
        }
        throw new IOException("Seçilen sürücüde yazılabilir bir test klasörü bulunamadı. Sürücünün yazma iznini ve yazma korumasını kontrol edin.");
    }
    static double[] Benchmark(string path, CancellationToken token, IProgress<Tuple<int,string>> progress)
    {
        IntPtr buffer=VirtualAlloc(IntPtr.Zero,(UIntPtr)Block,0x3000,4);
        if(buffer==IntPtr.Zero) throw new System.ComponentModel.Win32Exception();
        try
        {
            byte[] data=new byte[Block]; new Random().NextBytes(data); Marshal.Copy(data,0,buffer,Block);
            double[] speeds=new double[2];
            for(int phase=0;phase<2;phase++)
            {
                token.ThrowIfCancellationRequested();
                // NO_BUFFERING bypasses the Windows file cache. WRITE_THROUGH requests durable writes.
                using(SafeFileHandle file=CreateFile(path,phase==0?0x40000000u:0x80000000u,0,IntPtr.Zero,phase==0?1u:3u,0x20000000u|0x80000000u|0x08000000u,IntPtr.Zero))
                {
                    if(file.IsInvalid) throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
                    Stopwatch clock=Stopwatch.StartNew();
                    for(int i=0;i<1024;i++)
                    {
                        token.ThrowIfCancellationRequested(); int count;
                        bool ok=phase==0?WriteFile(file,buffer,Block,out count,IntPtr.Zero):ReadFile(file,buffer,Block,out count,IntPtr.Zero);
                        if(!ok) throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
                        if(count!=Block) throw new IOException("Disk eksik veri aktardı.");
                        if(i%8==0 || i==1023) progress.Report(Tuple.Create((phase*1024+i+1)*100/2048,(phase==0?"Yazılıyor":"Okunuyor")+": "+(i+1)+" / 1.024 MB"));
                    }
                    clock.Stop(); speeds[phase]=Total/clock.Elapsed.TotalSeconds/1000000.0;
                }
            }
            return speeds;
        }
        finally { VirtualFree(buffer,UIntPtr.Zero,0x8000); }
    }
    [STAThread] static void Main() { Application.EnableVisualStyles(); Application.SetCompatibleTextRenderingDefault(false); Application.Run(new DiskTest()); }
}
class MaterialCard : Panel
{
    public Color Edge=Color.Cyan;
    public MaterialCard() { DoubleBuffered=true; }
    protected override void OnPaintBackground(PaintEventArgs e) {
        var form=Parent as DiskTest; if(form!=null) form.PaintBackdrop(e.Graphics,-Left,-Top);
        e.Graphics.SmoothingMode=SmoothingMode.AntiAlias;
        int radius=(int)(24*e.Graphics.DpiX/96);
        using(var path=Shape.Round(new Rectangle(0,0,Width-1,Height-1),radius)) {
            using(var brush=new LinearGradientBrush(ClientRectangle,Color.FromArgb(13,29,41),Color.FromArgb(9,18,30),70f)) e.Graphics.FillPath(brush,path);
            using(var pen=new Pen(Color.FromArgb(140,Edge),1.4f)) e.Graphics.DrawPath(pen,path);
        }
    }
    protected override void OnResize(EventArgs e) { base.OnResize(e); using(var path=Shape.Round(ClientRectangle,20)) Region=new Region(path); }
}
class MaterialProgress : Control
{
    int current;
    public int Value { get { return current; } set { current=value; Invalidate(); } }
    public MaterialProgress() { DoubleBuffered=true; SetStyle(ControlStyles.SupportsTransparentBackColor,true); }
    protected override void OnPaint(PaintEventArgs e) {
        e.Graphics.SmoothingMode=SmoothingMode.AntiAlias;
        for(int i=0;i<40;i++) {
            int x=i*Width/40,w=Math.Max(3,Width/40-4);
            using(var path=Shape.Round(new Rectangle(x,1,w,Height-2),Math.Min(w,Height-2)))
            using(var brush=new SolidBrush(i*100/40<current?Color.FromArgb(255,79,65):Color.FromArgb(35,70,77))) e.Graphics.FillPath(brush,path);
        }
    }
}
class MaterialButton : Control
{
    bool hover,pressed,keyboardPressed;
    public MaterialButton() {
        Cursor=Cursors.Hand; Font=new Font("Bahnschrift",12,FontStyle.Bold); TabStop=true; AccessibleRole=AccessibleRole.PushButton;
        SetStyle(ControlStyles.UserPaint|ControlStyles.AllPaintingInWmPaint|ControlStyles.OptimizedDoubleBuffer|ControlStyles.Selectable|ControlStyles.ResizeRedraw,true);
        SetStyle(ControlStyles.StandardClick|ControlStyles.StandardDoubleClick,false);
    }
    protected override void OnPaintBackground(PaintEventArgs e) { }
    bool Hit(Point point) { using(var path=Shape.Round(new Rectangle(0,0,Width-1,Height-1),Height-1)) return path.IsVisible(point); }
    protected override void OnMouseMove(MouseEventArgs e) { base.OnMouseMove(e); bool next=Hit(e.Location); if(hover!=next) { hover=next; Invalidate(); } }
    protected override void OnMouseEnter(EventArgs e) { base.OnMouseEnter(e); hover=true; Invalidate(); }
    protected override void OnMouseLeave(EventArgs e) { base.OnMouseLeave(e); hover=false; Invalidate(); }
    protected override void OnMouseDown(MouseEventArgs e) { base.OnMouseDown(e); if(Enabled && e.Button==MouseButtons.Left && Hit(e.Location)) { Focus(); pressed=true; Capture=true; Invalidate(); } }
    protected override void OnMouseUp(MouseEventArgs e) {
        base.OnMouseUp(e); if(e.Button!=MouseButtons.Left) return;
        bool click=pressed && Enabled && Hit(e.Location); pressed=false; Capture=false; Invalidate(); if(click) OnClick(EventArgs.Empty);
    }
    protected override void OnMouseCaptureChanged(EventArgs e) { base.OnMouseCaptureChanged(e); if(!Capture) { pressed=false; Invalidate(); } }
    protected override void OnKeyDown(KeyEventArgs e) { base.OnKeyDown(e); if(Enabled && (e.KeyCode==Keys.Space || e.KeyCode==Keys.Enter)) { keyboardPressed=true; Invalidate(); e.SuppressKeyPress=true; } }
    protected override void OnKeyUp(KeyEventArgs e) { base.OnKeyUp(e); if(keyboardPressed && (e.KeyCode==Keys.Space || e.KeyCode==Keys.Enter)) { keyboardPressed=false; Invalidate(); if(Enabled) OnClick(EventArgs.Empty); e.Handled=true; } }
    protected override void OnGotFocus(EventArgs e) { base.OnGotFocus(e); Invalidate(); }
    protected override void OnLostFocus(EventArgs e) { base.OnLostFocus(e); keyboardPressed=false; Invalidate(); }
    protected override void OnEnabledChanged(EventArgs e) { base.OnEnabledChanged(e); hover=pressed=keyboardPressed=false; Invalidate(); }
    protected override void OnPaint(PaintEventArgs e) {
        var form=Parent as DiskTest; if(form!=null) form.PaintBackdrop(e.Graphics,-Left,-Top);
        e.Graphics.SmoothingMode=SmoothingMode.AntiAlias;
        e.Graphics.TextRenderingHint=TextRenderingHint.ClearTypeGridFit;
        Color fill=Enabled?BackColor:Color.FromArgb(14,28,35);
        if(Enabled && hover) fill=ControlPaint.Light(fill,0.13f);
        if(Enabled && ((pressed && hover)||keyboardPressed)) fill=ControlPaint.Dark(BackColor,0.12f);
        using(var path=Shape.Round(new Rectangle(0,0,Width-1,Height-1),Height-1)) {
            using(var brush=new LinearGradientBrush(ClientRectangle,ControlPaint.Light(fill,0.08f),fill,90f)) e.Graphics.FillPath(brush,path);
            using(var pen=new Pen(Enabled?ControlPaint.Light(fill,0.2f):Color.FromArgb(38,65,72))) e.Graphics.DrawPath(pen,path);
        }
        using(var brush=new SolidBrush(Enabled?ForeColor:Color.FromArgb(130,125,135)))
        using(var format=new StringFormat { Alignment=StringAlignment.Center,LineAlignment=StringAlignment.Center }) e.Graphics.DrawString(Text,Font,brush,ClientRectangle,format);
        if(Focused) { using(var pen=new Pen(Color.FromArgb(100,240,220))) using(var path=Shape.Round(new Rectangle(3,3,Width-7,Height-7),Height-7)) e.Graphics.DrawPath(pen,path); }
    }
}
static class Shape
{
    public static GraphicsPath Round(Rectangle r,int d) { var p=new GraphicsPath(); p.AddArc(r.X,r.Y,d,d,180,90); p.AddArc(r.Right-d,r.Y,d,d,270,90); p.AddArc(r.Right-d,r.Bottom-d,d,d,0,90); p.AddArc(r.X,r.Bottom-d,d,d,90,90); p.CloseFigure(); return p; }
}



