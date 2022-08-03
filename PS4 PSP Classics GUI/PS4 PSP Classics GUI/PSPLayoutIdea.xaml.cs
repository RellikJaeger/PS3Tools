﻿using DiscUtils.Iso9660;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Deployment.Application;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Xml;

namespace PS4_PSP_Classics_GUI
{
    /// <summary>
    /// Interaction logic for PSPLayoutIdea.xaml
    /// </summary>
    public partial class PSPLayoutIdea : Window
    {
        #region << Vars >>

        //Dialogs
        System.Windows.Forms.FolderBrowserDialog tempkeeper = null;

        //advanced window
        Advanced advanced = new Advanced();

        //VLC controls
        Vlc.DotNet.Forms.VlcControl tempvlc;

        //Background Workers
        private readonly BackgroundWorker bgWorkerVLC = new BackgroundWorker();
        private readonly BackgroundWorker bgWorkerSS = new BackgroundWorker();
        private readonly BackgroundWorker backgroundWorker1 = new BackgroundWorker();

        //Lists 
        public static List<string> pspfiles = new List<string>();

        //Strings
        public string PSPID = "";
        public static string xmlcontentid { get; set; }

        //Bools
        public static bool bgwClose;
        public static bool BusyCoping;

        //WPF Controls
        TextBox txtContentID = new TextBox();

        //XML 
        XmlDataDocument xmldoc = null;

        #region <<File Types and Instancase >>

        PSP_Tools.Pbp pbp = new PSP_Tools.Pbp();

        FileType.FileTypes type = FileType.FileTypes.Unknown;

        #endregion << File Tyoes and Instances >>

        #endregion << Vars >>

        public PSPLayoutIdea()
        {
            InitializeComponent();
        }

        #region << Events >>

        private void MediaPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            MediaPlayer.Visibility = Visibility.Hidden;

            //ask user if they want to open a menu to load a pbp cso or a game
        }

        BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.MakeTransparent(System.Drawing.Color.FromArgb(208, 208, 208));
                bitmap.MakeTransparent(System.Drawing.Color.Black);
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

         

            #region << Quick Sound/Video Extract >>


            System.IO.File.WriteAllBytes(AppCommonPath() + "PS4.mp3", Properties.Resources.ps4BGM);
            System.IO.File.WriteAllBytes(AppCommonPath() + "PSP.mp4", Properties.Resources.PSP_Logo);
            System.IO.File.WriteAllBytes(AppCommonPath() + "PSPWave.mp4", Properties.Resources.PSP_Wave);

            if (!Directory.Exists(AppCommonPath() + @"\PSPEmu\"))
            {
                UpdateInfo("Created Directory" + AppCommonPath() + @"\PSPEmu\");
                Directory.CreateDirectory(AppCommonPath() + @"\PSPEmu\");
            }
            System.IO.File.WriteAllBytes(AppCommonPath() + @"\PSPEmu\" + "param.sfo", Properties.Resources.param);

            #endregion <<Quick Sound/Video Extract >>

            #region << PSP Gim Files Extraction >>


            PSP_Tools.GIM gimmy = new PSP_Tools.GIM(Properties.Resources.tex_system2);

            var images = gimmy.ConvertToBitmaps();




            btnPhoto.Source = BitmapToImageSource(images[0]);

            #endregion << PSP Gim Files Extraction >>

            #region << Background Workers >>

            bgWorkerVLC.DoWork += BgWorkerVLC_DoWork;
            bgWorkerVLC.RunWorkerCompleted += BgWorkerVLC_RunWorkerCompleted;

            bgWorkerSS.DoWork += bgWorkerSS_DoWork;
            bgWorkerSS.WorkerSupportsCancellation = true;

            backgroundWorker1.DoWork += BackgroundWorker1_DoWork;
            backgroundWorker1.RunWorkerCompleted += backgroundWorker1_RunWorkerCompleted;

            #endregion << Background Workers >>

            #region << Boot Screen Settings >>

            if (Properties.Settings.Default.EnableBootScreen == true)
            {
                MediaPlayer.Visibility = Visibility.Visible;
                MediaPlayer.Source = new Uri(AppCommonPath() + "PSP.mp4");
                
                MediaPlayer.Play();
                MediaPlayer.MediaEnded += MediaPlayer_MediaEnded;
            }
            Wave.Source = new Uri(AppCommonPath() + "PSPWave.mp4");
            Wave.Play();

            #endregion << Boot Screen Settings >>

            #region << Gui Music >>

            if (Properties.Settings.Default.EnableGuiMusic == true)
            {
                //btnMutePlaySound.Background = ImageBrushFromBitmap(Properties.Resources.icon_sound_mute);
                SoundClass.PlayPS4Sound(SoundClass.Sound.PS4_Music);
            }

            #endregion << Gui Music >>

            #region << Version Numbering >>

            Version v = Assembly.GetExecutingAssembly().GetName().Version;
            //Check to see if we are ClickOnce Deployed.
            //i.e. the executing code was installed via ClickOnce
            if (ApplicationDeployment.IsNetworkDeployed)
            {
                //Collect the ClickOnce Current Version
                v = ApplicationDeployment.CurrentDeployment.CurrentVersion;
            }

            //Show the version in a simple manner
            this.Title = string.Format("PSPHD GUI Version : {0}", v);

            

            #endregion << Version Numbering >>

            #region << Advanced Window >>

            // i removed this as its no longer needed
            if (Properties.Settings.Default.EnableAdvancedMode == true)
            {
                advanced.Show();
            }


            #endregion << Advanced Window >>

            #region << Begin Maintaining the form and getting everything ready >>

            //quickly read sfo 
            UpdateInfo("Reading Custom SFO");
            PS2ClassicsSfo.SFO sfo = new PS2ClassicsSfo.SFO(AppCommonPath() + @"\PSPEmu\" + "param.sfo");

            UpdateInfo("Setting Content ID");
            //all we want to change is the Content ID which will rename the package 
            txtContentID.Text = sfo.ContentID.ToString().Trim().Substring(7, 9);


            #endregion << Begin Maintaining the form and getting everything ready >>

            #region << PMF and AT3 >>
            bgWorkerVLC.RunWorkerAsync();
            #endregion << PMF and AT3 >>
        }

        private void BgWorkerVLC_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }

        private void BgWorkerVLC_DoWork(object sender, DoWorkEventArgs e)
        {
            #region << PMF and AT3 >>
            if (Directory.Exists(AppCommonPath() + "libvlc"))
            {
                try
                {


                    var vlcLibDirectory = new DirectoryInfo(Path.Combine(AppCommonPath(), "libvlc", IntPtr.Size == 4 ? "win-x86" : "win-x64"));

                    //for testing do this 
                    //var vlcLibDirectory = new DirectoryInfo(AppCommonPath());
                    System.Windows.Application.Current.Dispatcher.Invoke(
        DispatcherPriority.Normal,
        (ThreadStart)delegate
        {

            tempvlc = new Vlc.DotNet.Forms.VlcControl();
            tempvlc.BeginInit();
            tempvlc.VlcLibDirectory = vlcLibDirectory;// /*libvlc's directory*/;
                                                      //this.MyControl.VlcMediaplayerOptions = new[] { "-vv" };

        string[] options =
            new string[] { "input-repeat=65535" };
            var mediaOptions = new string[] { "input-repeat=-1" };
            tempvlc.VlcMediaplayerOptions = options;
            Views.VLC._WindowsFormsHost.Child = tempvlc;



        //this.WindowsFormsHost.Child = tempvlc;

        tempvlc.EndInit();

            Util.SoundClass.atr3vlc = new Vlc.DotNet.Forms.VlcControl();
            Util.SoundClass.atr3vlc.BeginInit();
            Util.SoundClass.atr3vlc.VlcLibDirectory = vlcLibDirectory;// /*libvlc's directory*/;
                                                                      //this.MyControl.VlcMediaplayerOptions = new[] { "-vv" };
        Util.SoundClass.atr3vlc.EndInit();

        });

                }
                catch (Exception ex)
                {

                }
            }
            #endregion << PMF and AT3 >>
        }

        /// <summary>
        /// Background Worker For Progres Bar Screen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bgWorkerSS_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke((Action)delegate {
                    // your code

                    Busy Busy = new Busy(bgWorkerSS);
                    Busy.ShowDialog();
                    Busy.Focus();
                });
            }
            catch (Exception ex)
            {

            }
        }

        private void txtContentID_Copy_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            Icon0.Visibility = Visibility.Visible;
            try
            {
                if (Util.SoundClass.atr3vlc.IsPlaying)
                {
                    Util.SoundClass.atr3vlc.Stop();
                }
                if (tempvlc.IsPlaying)
                {
                    tempvlc.Stop();
                }
            }
            catch
            {

            }
            if (Properties.Settings.Default.EnablePMF == false)
            {
                WindowsFormsHost.Visibility = Visibility.Hidden;
            }
            else
            {
                //untill current issue is resolved 
                WindowsFormsHost.Visibility = Visibility.Hidden;
                Icon0.Visibility = Visibility.Visible;
            }
            //UpdateInfo("Checking if there is already a mutli iso screen open and closing it");

            //close open form 
            //this closes any open form
            if (Application.Current.Windows.OfType<MultipleISO_s>().Count() == 1)
                Application.Current.Windows.OfType<MultipleISO_s>().First().Close();
            MultipleISO_s multi = new MultipleISO_s();
            //UpdateInfo("Open File Dialog");
            //Open File Dialog For ISO Files
            OpenFileDialog thedialog = new OpenFileDialog();
            thedialog.Title = "Select ISO";
            thedialog.Filter = "PSP Image File (*.iso,*.cso,*.pbp,*.ISO,*.CSO,*.PBP)|*.iso;*.cso;*.pbp;*.ISO;*.CSO;*.PBP";
            //"Plain text files (*.csv;*.txt)|*.csv;*.txt";
            thedialog.Multiselect = false;//psp emu only supports 1.8Gig so we might as well only allow one iso/pbp/cso file
            thedialog.InitialDirectory = Environment.SpecialFolder.MyComputer.ToString();

            if (thedialog.ShowDialog() == true)
            {

                //get the file type 
                type = FileType.LoadFileInfo(thedialog.FileName);

                if (type == FileType.FileTypes.Unknown)
                {
                    MessageBox ps4messagebox = new MessageBox("Unknown File Type Selected", "File Type Unknown", PS4_MessageBoxButton.OK, SoundClass.Sound.Error);
                    ps4messagebox.ShowDialog();
                    return;
                }

                if (type == FileType.FileTypes.CSO)
                {
                    UpdateString("Decompressing CISO");

                    Thread threadholder = null ;

                    new Thread(() =>
                    {
                        Thread.CurrentThread.IsBackground = true;
                        /* run your code here */
                        threadholder = Thread.CurrentThread;

                        //we will have to convert cso to iso first no way around this currently 
                        PSP_Tools.UMD.CISO.DecompressCSO(thedialog.FileName, AppCommonPath() + "\\Working\\tempiso");

                        //once decompressed fire up the new type as iso and replace file name;
                        thedialog.FileName = AppCommonPath() + "\\Working\\tempiso";
                        type = FileType.FileTypes.ISO;

                    }).Start();
                    while(threadholder == null)
                    {
                        //DoEvents();
                    }
                    while (threadholder.ThreadState != System.Threading.ThreadState.Stopped)
                    {
                        //Thread.Sleep(100);
                        //dont know what to do here
                    }
                    OpenCloseWaitScreen(false);
                }

                if (type == FileType.FileTypes.PBP)
                {
                    pbp = new PSP_Tools.Pbp();
                    pbp.LoadPbp(thedialog.FileName);

                    string DiscID = pbp.GetPBPDiscID();

                    pspfiles = thedialog.FileNames.ToList<string>();

                    byte[] array = pbp.ReadFileFromPBP(PSP_Tools.Pbp.DataType.ParamSfo);
                    if (array != null)
                    {
                        PSP_Tools.PARAM_SFO sfo = new PSP_Tools.PARAM_SFO(array);
                        if (sfo.DISC_ID != "")
                        {
                            txtContentID.Text = sfo.DISC_ID;//disc id of PSP Game
                            PSPID = sfo.DISC_ID;
                        }
                        txtTitle.Text = sfo.Title;
                        byte[] Icon0Png = pbp.ReadFileFromPBP(PSP_Tools.Pbp.DataType.Icon0Png);
                        if (Icon0Png.Length != 0)
                        {
                            Icon0.Source = ToImage(Icon0Png);
                        }

                        byte[] Pic1Png = pbp.ReadFileFromPBP(PSP_Tools.Pbp.DataType.Pic1Png);
                        if (Pic1Png.Length != 0)
                        {
                            ImageBrush imgB = new ImageBrush();

                            BitmapImage btpImg = new BitmapImage();
                            btpImg.BeginInit();
                            btpImg.StreamSource = new MemoryStream(Pic1Png);
                            btpImg.EndInit();
                            imgB.ImageSource = btpImg;

                            xmbOverlay.Source = btpImg;
                        }
                    }
                    /*Data.psar = boot.bin ;)
                      data.psp = eboot.bin*/

                    //pbp.WritePDPFiles(AppCommonPath() + @"\PSP\",pspdata:"EBOOT.BIN",psrdata:"BOOT.BIN");
                    //we should probably only write out the data when the item is completed 

                    if (Properties.Settings.Default.EnablePMF == true)
                    {
                        ////play at3
                        byte[] atrac3 = pbp.ReadFileFromPBP(PSP_Tools.Pbp.DataType.Snd0At3);
                        if (atrac3.Length != 0)
                        {
                            Util.SoundClass.atr3vlc.Play(new MemoryStream(atrac3));
                            // Util.SoundClass.Init_SoundPlayer(atrac3);
                        }

                        byte[] pmf = pbp.ReadFileFromPBP(PSP_Tools.Pbp.DataType.Icon1Pmf);
                        if (pmf.Length != 0)
                        {
                            tempvlc.Play(new MemoryStream(pmf));
                            // Util.SoundClass.Init_SoundPlayer(atrac3);
                        }
                    }
                }

                else if (type == FileType.FileTypes.ISO)
                {
                    //UpdateInfo("adding iso files to list");
                    pspfiles = thedialog.FileNames.ToList<string>();//tada now we know how many iso's there is 
                    if (pspfiles.Count > 1)
                    {

                        System.Windows.MessageBox.Show("Maximum amount of ISO's allowed by the PS4 in a PSPHD is 1");
                        pspfiles.Clear();
                        return;
                    }
                    if (pspfiles.Count == 1)
                    {
                        //UpdateInfo("Standard ISO Method");
                        #region << For Single File >>
                        //set the path and the text on the gui
                        string isopath = thedialog.FileName;

                        //now using the file stream we can read the CNF file
                        using (FileStream isoStream = File.OpenRead(isopath))
                        {
                            //use disk utils to read iso quickly
                            CDReader cd = new CDReader(isoStream, true);
                            //look for the spesific file(I Wanted to use UMD_DATA.BIN bit i decided not to since homebrew doesnt have that we should use sfo)
                            Stream fileStream = cd.OpenFile(@"PSP_GAME\PARAM.SFO", FileMode.Open);
                            // Use fileStream...
                            // Read SFO
                            PSP_Tools.PARAM_SFO sfo = new PSP_Tools.PARAM_SFO(fileStream);

                            string DiscID = sfo.DISC_ID.ToString();//read string to end this will read all the info we need


                            string PSPId = DiscID;

                            if (PSPId != string.Empty)
                            {
                                PSPID = PSPId.Replace(".", "");

                                if (Properties.Settings.Default.EnablePSPIDReplace == true)
                                {
                                    txtContentID.Text = PSPID.Replace("_", "");
                                }

                                txtTitle.Text = sfo.Title;

                                try
                                {
                                    Stream iconstream = cd.OpenFile(@"PSP_GAME\ICON0.PNG", FileMode.Open);

                                    byte[] Icon0Png = ReadFully(iconstream);
                                    if (Icon0Png.Length != 0)
                                    {
                                        Icon0.Source = ToImage(Icon0Png);
                                    }
                                }
                                catch (Exception ex)
                                {

                                }
                                try
                                {
                                    Stream iconstream = cd.OpenFile(@"PSP_GAME\PIC1.PNG", FileMode.Open);


                                    byte[] Pic1Png = ReadFully(iconstream);
                                    if (Pic1Png.Length != 0)
                                    {
                                        ImageBrush imgB = new ImageBrush();

                                        BitmapImage btpImg = new BitmapImage();
                                        btpImg.BeginInit();
                                        btpImg.StreamSource = new MemoryStream(Pic1Png);
                                        btpImg.EndInit();
                                        imgB.ImageSource = btpImg;

                                        xmbOverlay.Source = btpImg;
                                    }
                                }
                                catch (Exception ex)
                                {

                                }
                                try
                                {
                                    if (Properties.Settings.Default.EnablePMF == true)
                                    {

                                        ////play at3
                                        Stream atrac3stream = cd.OpenFile(@"PSP_GAME\SND0.AT3", FileMode.Open);


                                        byte[] atrac3 = ReadFully(atrac3stream);
                                        if (atrac3.Length != 0)
                                        {
                                            var mediaOptions = new string[] { "input-repeat=65535" };
                                            Util.SoundClass.atr3vlc.Play(new MemoryStream(atrac3), mediaOptions);
                                            // Util.SoundClass.Init_SoundPlayer(atrac3);

                                        }
                                    }
                                }
                                catch (Exception ex)
                                {

                                }

                                try
                                {
                                    if (Properties.Settings.Default.EnablePMF == true)
                                    {
                                        Stream pmfstream = cd.OpenFile(@"PSP_GAME\ICON1.PMF", FileMode.Open);
                                        //Icon0.Visibility = Visibility.Hidden;
                                        byte[] pmf = ReadFully(pmfstream);
                                        if (pmf.Length != 0)
                                        {
                                            //Icon0.Visibility = Visibility.Hidden;
                                            var mediaOptions = new string[] { "input-repeat=65535" };
                                            tempvlc.Play(new MemoryStream(pmf), mediaOptions);
                                            //tempvlc.Stopped += delegate
                                            //{

                                            //    var mediaOptions = new string[] { "input-repeat=65535" };
                                            //    tempvlc.Play(new MemoryStream(pmf),mediaOptions);
                                            //    tempvlc.Stopped += delegate
                                            //    {
                                            //        tempvlc.Play(new MemoryStream(pmf), mediaOptions);
                                            //    };
                                            //};
                                        }
                                    }
                                }
                                catch
                                {

                                }
                            }
                            else
                            {
                                MessageBoxResult msrlt = System.Windows.MessageBox.Show("Could not load PSP ID\n\n would you like to submit an issue ?", "Error Reporting", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes);
                                if (msrlt == MessageBoxResult.Yes)
                                {
                                    //load github issue page
                                    Process.Start(@"https://github.com/xXxTheDarkprogramerxXx/PS3Tools/issues");
                                }

                            }
                        }

                        #endregion << For Single File >>
                    }
                    else if (pspfiles.Count > 1)
                    {
                        //UpdateInfo("Multi ISO Method");
                        //UpdateInfo("Opening Mutli ISO Screen");
                        // multi.Show();
                        //multi.SetDesktopLocation(this.Location.X + this.Size.Width, this.Location.Y);
                        throw new Exception("Multi ISO not Supported!\n how did you even get tbhis far ?");
                    }
                }
            }
        }

        private void BackgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {

                System.Windows.Forms.FolderBrowserDialog saveFileDialog1 = tempkeeper;


                UpdateString("Creating Working Area");
                UpdateInfo("Creating Working Area : " + AppCommonPath() + @"\Working\");
                if (!Directory.Exists(AppCommonPath() + @"\Working\"))
                {
                    Directory.CreateDirectory(AppCommonPath() + @"\Working\");
                }

                UpdateString("Getting needed files");
                UpdateInfo("Getting needed files");

                //first we need to build the new SFO
                UpdateInfo("Copying " + AppCommonPath() + @"\PSPEmu\" + "sfo.xml to " + AppCommonPath() + @"\Working\" + "sfo.xml");
                File.Copy(AppCommonPath() + @"\PSPEmu\" + "sfo.xml", AppCommonPath() + @"\Working\" + "sfo.xml", true);

                //now we need to prase it and change it 

                UpdateString("Gathering GP4 Info");
                UpdateInfo("Gathering GP4 Info");


                //create new XML Document 
                xmldoc = new XmlDataDocument();
                //nodelist 
                XmlNodeList xmlnode;
                //load the xml file from the base directory
                UpdateInfo("Loading SFO as xml");
                xmldoc.Load(AppCommonPath() + @"\Working\" + "sfo.xml");
                //now load the nodes
                xmlnode = xmldoc.GetElementsByTagName("paramsfo");//volume is inside the xml
                UpdateInfo("Update Content ID and other ifno in SFO");                                           //loop to get all info from the node list
                foreach (XmlNode xn in xmlnode)
                {
                    XmlNode xNode = xn.SelectSingleNode("CONTENT_ID");
                    XmlNodeList nodes = xmldoc.SelectNodes("//param[@key='CONTENT_ID']");
                    if (nodes != null)
                    {

                        if (pspfiles.Count > 1)
                        {
                            System.Windows.Application.Current.Dispatcher.Invoke(
                       DispatcherPriority.Normal,
                       (ThreadStart)delegate
                       {
                           xmlcontentid = "UP9000-" + txtContentID.Text.Trim() + "_00-" + txtContentID.Text.Trim() + "0000001";//make this the same no ps2 id required
                       });
                            nodes[0].InnerText = xmlcontentid;
                        }
                        else
                        {
                            System.Windows.Application.Current.Dispatcher.Invoke(
                        DispatcherPriority.Normal,
                        (ThreadStart)delegate
                        {
                            xmlcontentid = "UP9000-" + txtContentID.Text.Trim() + "_00-" + PSPID.Replace("_", "") + "0000001";
                        });
                            nodes[0].InnerText = xmlcontentid;
                        }
                        //});
                    }
                    nodes = xmldoc.SelectNodes("//param[@key='TITLE']");
                    if (nodes != null)
                    {
                        System.Windows.Application.Current.Dispatcher.Invoke(
                        DispatcherPriority.Normal,
                        (ThreadStart)delegate
                        {
                            nodes[0].InnerText = txtTitle.Text.Trim();
                        });
                    }
                    nodes = xmldoc.SelectNodes("//param[@key='TITLE_ID']");
                    if (nodes != null)
                    {
                        System.Windows.Application.Current.Dispatcher.Invoke(
                        DispatcherPriority.Normal,
                        (ThreadStart)delegate
                        {
                            nodes[0].InnerText = txtContentID.Text.Trim();
                        });
                    }
                    for (int i = 1; i < 7; i++)
                    {
                        //fix the enter key issue i have found in some in
                        nodes = xmldoc.SelectNodes("//param[@key='SERVICE_ID_ADDCONT_ADD_" + i + "']");
                        if (nodes != null)
                        {
                            nodes[0].InnerText = string.Empty;
                        }
                    }
                }
                //save this into the working folder
                xmldoc.Save(AppCommonPath() + @"\Working\" + "sfo.xml");


                UpdateString("Creating GP4 Project");

                SaveGp4();


                UpdateString("Creating SFO File");

                #region << Orbis >>

                //we will just modify the orginal file its way way easier
                //also no need for SCE tools then
                //now call orbis and create sfo
                //Orbis_CMD("", "sfo_create \"" + AppCommonPath() + @"\Working\" + "sfo.xml" + "\" \"" + AppCommonPath() + @"\Working\" + "param.sfo" + "\"");

                //move SFO to main directory with locations of new images 

                //UpdateString("Moving SFO File");
                //File.Copy(AppCommonPath() + @"\Working\" + "param.sfo", AppCommonPath() + @"\PSP\sce_sys\param.sfo", true);


                #endregion << Orbis >>

                #region << PSFO Editor>>

                //Create the ContentID UP9000-

                System.Windows.Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate
                {
                    xmlcontentid = "UP9000-" + txtContentID.Text.Trim() + "_00-" + PSPID.Replace("_", "") + "0000001";
                });

                //Load The SFO From the existing File Location
                Param_SFO.PARAM_SFO psfo = new Param_SFO.PARAM_SFO(AppCommonPath() + @"\PSP\sce_sys\param.sfo");

                //Set Item Info
                for (int i = 0; i < psfo.Tables.Count; i++)
                {
                    if (psfo.Tables[i].Name == "CONTENT_ID")
                    {
                        var tempitem = psfo.Tables[i];
                        tempitem.Value = xmlcontentid.Trim();
                        psfo.Tables[i] = tempitem;
                    }
                    if (psfo.Tables[i].Name == "TITLE")
                    {
                        string title = "";
                        System.Windows.Application.Current.Dispatcher.Invoke(
                       DispatcherPriority.Normal,
                       (ThreadStart)delegate
                       {
                           title = txtTitle.Text.Trim();
                       });
                        var tempitem = psfo.Tables[i];
                        tempitem.Value = title.Trim();
                        psfo.Tables[i] = tempitem;
                    }

                    if (psfo.Tables[i].Name == "TITLE_ID")
                    {
                        string title = "";
                        System.Windows.Application.Current.Dispatcher.Invoke(
                       DispatcherPriority.Normal,
                       (ThreadStart)delegate
                       {
                           title = txtContentID.Text.Trim();
                       });
                        var tempitem = psfo.Tables[i];
                        tempitem.Value = title.Trim();
                        psfo.Tables[i] = tempitem;
                    }
                }

                //Save SFO
                psfo.SaveSFO(psfo, AppCommonPath() + @"\PSP\sce_sys\param.sfo");//save the sfo over the existing one

                #endregion << PSFO Editor>>

                #region << Save Images>>

                System.Windows.Application.Current.Dispatcher.Invoke(
                        DispatcherPriority.Normal,
                        (ThreadStart)delegate
                        {
                            BitmapSource sr = Icon0.Source as BitmapSource;
                            Bitmap converted = GetBitmap(sr);
                            converted = ResizeImage(converted, 512, 512);//converts the image to the correct size
                            converted = ConvertTo24bpp(converted);//converts image to 24bpp
                            converted.Save(AppCommonPath() + @"PSP\sce_sys\icon0.png", System.Drawing.Imaging.ImageFormat.Png);
                        });

                System.Windows.Application.Current.Dispatcher.Invoke(
                        DispatcherPriority.Normal,
                        (ThreadStart)delegate
                        {
                            //ImageBrush b = ;
                            BitmapSource src = (BitmapSource)xmbOverlay.Source;

                            Bitmap converted = GetBitmap(src);
                            converted = ResizeImage(converted, 1920, 1080);//converts the image to the correct size
                            converted = ConvertTo24bpp(converted);//converts image to 24bpp
                            converted.Save(AppCommonPath() + @"PSP\sce_sys\pic1.png", System.Drawing.Imaging.ImageFormat.Png);
                        });


                #endregion << Save Images>>

                #region << Custom PSP Confog >>

                UpdateString("Moving Custom PSP Config");
                //now we need to check the config file
                if (pspfiles.Count > 1)
                {
                    //modify config file 
                    var textfile = File.ReadAllText(AppCommonPath() + @"PSP\config-emu-ps4.txt");
                    if (textfile.Contains("--max-disc-num="))
                    {
                        //read the nesasary info
                        string Is = @"--max-disc-num=";

                        int start = textfile.ToString().IndexOf(Is) + Is.Length;
                        int end = start + 1;//cause we know its one char more
                        if (end > start)
                        {
                            string texttoreplace = textfile.ToString().Substring(start, end - start);
                            textfile = textfile.Replace(Is + texttoreplace, @"--max-disc-num=" + pspfiles.Count);
                        }
                    }

                    textfile = textfile.Replace(@"#--path-patches=""/app0/patches""", @"--path-patches=""/app0/patches""");//add patches
                    textfile = textfile.Replace(@"#--path-featuredata=""/app0/patches""", @"--path-featuredata=""/app0/patches""");//add featuredata
                    textfile = textfile.Replace(@"#--path-toolingscript=""/app0/patches""", @"--path-toolingscript=""/app0/patches""");//#--path-toolingscript=""/app0/patches"""
                    File.WriteAllText(AppCommonPath() + @"PSP\config-emu-ps4.txt", textfile);
                }

                #endregion << Custom PSP Confog >>

                #region << Enable Mysis Patches >>

                //these patches where orginally done by Mysis ... a c# version was made by Pink1
                //if (Properties.Settings.Default.EnableMysisPatch == true)
                //{
                //    if(type == FileType.FileTypes.PBP)
                //    {
                //        pbp.WritePBPFiles()
                //    }
                //}
                //this will be added after initial release and when ppspp is converted correctly
                #endregion << Enable Mysis Patches >>

                #region << Move ISO >>

                UpdateString("Moving ISO File This May Take Some Time");
                BusyCoping = true;
                new System.Threading.Thread(new System.Threading.ThreadStart(delegate
                {

                    if (MainWindow.pspfiles.Count > 1)

                        for (int i = 0; i < MainWindow.pspfiles.Count; i++)
                        {
                            {

                                UpdateString("Moving ISO File " + (i + 1) + "/" + MainWindow.pspfiles.Count + " This May Take Some Time");
                                File.Copy(pspfiles[i].ToString().Trim(), AppCommonPath() + @"\PS2\image\disc" + String.Format("{0:D2}", i + 1) + ".iso", true);

                            }
                        }
                    else
                    {
                        //we need to handle iso creation from PBP
                        if (type == FileType.FileTypes.PBP)
                        {

                            UpdateString("Extracting PBP");

                            //we need to extart all files to a folder within working 
                            //we want an eboot and a boot.bin so we create both (since the PS4 Boots boot.bin 
                            if (!Directory.Exists(AppCommonPath() + @"\Working\PSPISO\"))
                            {
                                Directory.CreateDirectory(AppCommonPath() + @"\Working\PSPISO\");
                            }
                            pbp.WritePBPFiles(AppCommonPath() + @"\Working\PSPISO\", pspdata: "EBOOT.BIN", psrdata: "DATA.BIN", make_eboot_boot: true);

                            //clean up blank file
                            File.Delete(AppCommonPath() + @"\PSP\DATA\USER_L0.IMG");

                            //now pack the pbp as an iso 
                            PSP_Tools.UMD.ISO umdiso = new PSP_Tools.UMD.ISO();
                            umdiso.PSPTitle = psfo.Title;//set the title of the iso to that which is inside the sfo
                            umdiso.CreateISO(AppCommonPath() + @"\Working\PSPISO\", AppCommonPath() + @"\PSP\DATA\USER_L0.IMG", false);//fake sign should not have to apply here 

                            UpdateString("Creating ISO");

                            while (umdiso.Status == PSP_Tools.UMD.ISO.ISOStatus.Busy)
                            {
                                //sleep the thread
                                DoEvents();
                            }
                            if (umdiso.Status == PSP_Tools.UMD.ISO.ISOStatus.Completed)
                            {
                                //BusyCoping = false;
                            }
                        }

                        if (type == FileType.FileTypes.ISO)
                        {
                            //clean up the blank file
                            File.Delete(AppCommonPath() + @"\PSP\DATA\USER_L0.IMG");
                            string currentimage = "";
                            currentimage = pspfiles[0].ToString().Trim();//first and only item
                            File.Copy(currentimage, AppCommonPath() + @"\PSP\DATA\USER_L0.IMG", true);

                            //BusyCoping = false;
                        }
                    }

                    //check if file is encrypted inside the system 
                    //if it is decrypt it
                    bool encrypted = false;

                    byte[] EBOOT;
                    byte[] EBOOTDec;
                    using (FileStream isoStream = File.OpenRead(AppCommonPath() + @"\PSP\DATA\USER_L0.IMG"))
                    {
                        UpdateString("Eboot is encrypted\nDecrypting....");


                        //use disk utils to read iso quickly
                        CDReader cd = new CDReader(isoStream, true);
                        //look for the spesific file
                        Stream fileStream = cd.OpenFile("\\PSP_GAME\\SYSDIR\\EBOOT.BIN", FileMode.Open);
                        // Use fileStream...
                        EBOOT = new byte[fileStream.Length];
                        EBOOTDec = new byte[fileStream.Length];
                        fileStream.Read(EBOOT, 0, (int)fileStream.Length);
                        //File.WriteAllBytes(AppCommonPath() + @"\eboot.bin", buffer);
                        fileStream.Position = 0;
                        using (BinaryReader binaryReader = new BinaryReader(fileStream))
                        {
                            Byte[] FileHeader = binaryReader.ReadBytes(4);

                            if (FileHeader.SequenceEqual(new byte[4] { 0x7E, 0x50, 0x53, 0x50 }))
                            {
                                //file is encrypted decrypt it
                                encrypted = true;

                                //new uses C# no more writing disgusting files

                                PSP_Tools.Crypto.EncryptedPrx prxdecrypter = new PSP_Tools.Crypto.EncryptedPrx();
                                EBOOTDec = prxdecrypter.Decrypt(EBOOT, true);

                                #region << OLD >>
                                //OLD USED the PRX Decryption tool by https://github.com/John-K/pspdecrypt

                                //System.IO.File.WriteAllBytes(AppCommonPath() + @"\decrypt.exe", Properties.Resources.pspdecrypt);
                                //ProcessStartInfo start = new ProcessStartInfo();
                                //start.FileName = AppCommonPath() + "decrypt.exe";
                                //start.Arguments = @"""" + AppCommonPath() + @"eboot.bin""";//same folder easy stuff
                                //start.UseShellExecute = false;
                                //start.RedirectStandardOutput = true;
                                //start.CreateNoWindow = true;

                                //using (Process process = Process.Start(start))
                                //{
                                //    process.ErrorDataReceived += Process_ErrorDataReceived;
                                //    using (StreamReader reader = process.StandardOutput)
                                //    {
                                //        string result = reader.ReadToEnd();

                                //    }
                                //}
                                //Thread.Sleep(110);
                                ////cleanup
                                //File.Delete(AppCommonPath() + "decrypt.exe");


                                //var index = BinarryTools.IndexOf(isoStream, buffer);

                                #endregion << OLD >>

                                //now try and fix load table

                                EBOOTDec = PSP_Tools.PS4.LoadElf(EBOOTDec);


                            }
                        }

                        //always clean up
                        //File.Delete(AppCommonPath() + @"\eboot.bin");
                    }
                    if (encrypted == true)
                    {
                        UpdateString("Extracting disc and recreating....");
                        string Label = "";




                        using (FileStream ISOStream = File.Open(AppCommonPath() + @"\PSP\DATA\USER_L0.IMG", FileMode.Open))
                        {
                            CDReader Reader = new CDReader(ISOStream, true, true);
                            //BinarryTools.ExtractDirectory(Reader.Root, AppCommonPath() + "\\" + Path.GetFileNameWithoutExtension(AppCommonPath() + @"\PSP\DATA\USER_L0.IMG") + "\\", "");


                            Label = Reader.VolumeLabel;

                            Reader.Dispose();
                        }

                        long num = EBOOT.Length;
                        if (num > 512320L)
                        {
                            num = 512320L;
                        }


                        byte[] query = (byte[])BinarryTools.ReadRomData(EBOOT, 0L, (int)num);


                        var offset = BinarryTools.FindOffset(AppCommonPath() + @"\PSP\data\USER_L0.IMG", query);
                        byte[] value = (byte[])BinarryTools.ReadRomData(EBOOTDec, 0L, (int)EBOOT.Length);


                        //now replace
                        FileStream fileStream = new FileStream(AppCommonPath() + @"\PSP\data\USER_L0.IMG", FileMode.Open, FileAccess.Write, FileShare.Write);
                        fileStream.Seek(Convert.ToInt64(offset), SeekOrigin.Begin);
                        fileStream.Write(value, 0, value.Length);
                        fileStream.Close();
                        //File.Delete(AppCommonPath() + @"\eboot.bin");
                        //File.Delete(AppCommonPath() + @"\eboot.bin.dec");//delete and cleanup
                        //now replace file 
                        //Replace BOOT.BIN with Eboot.BIn
                        //File.Copy(AppCommonPath() + @"\eboot.bin.dec", AppCommonPath() + "\\" + Path.GetFileNameWithoutExtension(AppCommonPath() + @"\PSP\DATA\USER_L0.IMG") + @"\PSP_GAME\SYSDIR\EBOOT.BIN", true);
                        //File.Copy(AppCommonPath() + @"\eboot.bin.dec", AppCommonPath() + "\\" + Path.GetFileNameWithoutExtension(AppCommonPath() + @"\PSP\DATA\USER_L0.IMG") + @"\PSP_GAME\SYSDIR\BOOT.BIN", true);
                        //File.Delete(AppCommonPath() + @"\eboot.bin.dec");//delete and cleanup
                        //re-create the iso
                        //BinarryTools.CreateIsoImage(AppCommonPath() + "\\" + Path.GetFileNameWithoutExtension(AppCommonPath() + @"\PSP\DATA\USER_L0.IMG"), AppCommonPath() + @"\PSP\DATA\USER_L0.IMG", Label);



                        //PSP_Tools.UMD.ISO iso = new PSP_Tools.UMD.ISO();
                        //iso.PSPTitle = Label ?? psfo.Title;

                        //iso.CreateISO(AppCommonPath() + "\\" + Path.GetFileNameWithoutExtension(AppCommonPath() + @"\PSP\DATA\USER_L0.IMG"), AppCommonPath() + @"\PSP\DATA\USER_L0.IMG");


                        //PSP_Tools.UMD.Sign.UMDSIGN()



                        //while (iso.Status == PSP_Tools.UMD.ISO.ISOStatus.Busy)
                        //{
                        //    //sleep the thread
                        //    DoEvents();
                        //}
                        //if (iso.Status == PSP_Tools.UMD.ISO.ISOStatus.Completed)
                        {
                            //DeleteDirectory(AppCommonPath() + "\\" + Path.GetFileNameWithoutExtension(AppCommonPath() + @"\PSP\DATA\USER_L0.IMG"));
                        }



                    }


                    BusyCoping = false;
                })).Start();

                while (BusyCoping == true)
                {
                    DoEvents();
                }
                #endregion << Move ISO >>

                #region << Patch NP Title File >>
                if (File.Exists(AppCommonPath() + @"\PSP\sce_sys\nptitle.dat"))
                {
                    //updatenptitledata
                    UpdateString("Patching NP Title");
                    // Original Byte string to find and Replace "43 55 53 41 30 35 32 38 39 5F 30 30"
                    Stream FileStream = new FileStream(AppCommonPath() + @"\PSP\sce_sys\nptitle.dat", FileMode.Open, FileAccess.Read, FileShare.Read);

                    //Read NPTitle ID
                    FileStream.Seek(16, SeekOrigin.Begin);
                    byte[] array = new byte[9];
                    FileStream.Read(array, 0, array.Length);
                    //Close the stream
                    FileStream.Close();

                    //get the current stream value
                    var currentstr = Encoding.ASCII.GetString(array);
                    string contentid = currentstr;
                    System.Windows.Application.Current.Dispatcher.Invoke(
                            DispatcherPriority.Normal,
                            (ThreadStart)delegate
                            {
                                contentid = txtContentID.Text;
                            });
                    //encode to bytes the new content id
                    var bytes = Encoding.ASCII.GetBytes(contentid);

                    //read current bytes from file
                    byte[] file = File.ReadAllBytes(AppCommonPath() + @"\PSP\sce_sys\nptitle.dat");

                    //and replace
                    int x, j, iMax = file.Length - array.Length;
                    for (x = 0; x <= iMax; x++)
                    {
                        for (j = 0; j < array.Length; j++)
                            if (file[x + j] != array[j]) break;
                        if (j == array.Length) break;
                    }
                    if (x <= iMax)
                    {
                        for (j = 0; j < array.Length; j++)
                            file[x + j] = bytes[j];
                        File.WriteAllBytes(AppCommonPath() + @"\PSP\sce_sys\nptitle.dat", file);
                    }
                }

                #endregion << Patch NP Title File >>

                UpdateString("Creating PS4 PKG");
                BusyCoping = true;
                new System.Threading.Thread(new System.Threading.ThreadStart(delegate
                {
                    Orbis_CMD("", "img_create --oformat pkg \"" + AppCommonPath() + @"\PSPEmu\" + "PSPClassics.gp4\" \"" + saveFileDialog1.SelectedPath + "\"");
                    BusyCoping = false;
                })).Start();

                while (BusyCoping == true)
                {
                    DoEvents();
                }

                UpdateString("Done Opening Location");
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }

        public static string ByteArrayToString(byte[] ba)
        {
            System.Text.StringBuilder hex = new System.Text.StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        /// <summary>
        /// When Background Worker Is Done Open folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            System.Windows.Forms.FolderBrowserDialog saveFileDialog1 = tempkeeper;

            //MessageBox.Show("Convert completed");
            //no messagebox instead play the bootlogo sound 

            SoundClass.PlayPS4Sound(SoundClass.Sound.Notification);
            OpenCloseWaitScreen(false);
            Process.Start(saveFileDialog1.SelectedPath);


            //now we delete the working directory
            DeleteDirectory(AppCommonPath() + @"\Working\");
            DeleteDirectory(AppCommonPath() + @"\PSP\");
            DeleteDirectory(AppCommonPath() + @"\PSPEmu\");

            //Delete Some FIles that are no longer required
            File.Delete(AppCommonPath() + @"\pkg.exe");
            File.Delete(AppCommonPath() + @"\orbis-pub-cmd.exe");


            UpdateInfo("Ready");
        }

        private void Icon_Drop(object sender, DragEventArgs e)
        {

        }

        private void psBUtton_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
            this.Close();
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SoundClass.PlayPS4Sound(SoundClass.Sound.PS4_Info_Pannel_Sound);
                SettingsWindow settings = new SettingsWindow();
                settings.ShowDialog();
            }
            catch (Exception ex)
            {

            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {

        }


        private void btnX_Click(object sender, RoutedEventArgs e)
        {
            if (pspfiles.Count == 0)
            {
                return;
            }

            UpdateInfo("Extarcting Resources Started");
            //extract all resources for the current program
            ExtractAllResources();


            UpdateInfo("Converting ISO(s) to PKG ");

            CheckString();

            //moving code over
            ExtractAllResources();//extarct all resources when we need it

            UpdateInfo("Save File Dialog");

            System.Windows.Forms.FolderBrowserDialog saveFileDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            //saveFileDialog1.Filter = "PS4 PKG|*.pkg";
            //saveFileDialog1.Title = "Save an PS4 PKG File";
            //saveFileDialog1.ov
            if (System.Windows.Forms.DialogResult.OK != saveFileDialog1.ShowDialog())
            {
                return;
            }

            tempkeeper = saveFileDialog1;
            try
            {
                OpenCloseWaitScreen(true);
                if (backgroundWorker1.IsBusy == false)
                {
                    backgroundWorker1.RunWorkerAsync();
                }
            }
            catch (Exception ex)
            {
                OpenCloseWaitScreen(false);
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        private void btnSettings_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SettingsWindow settings = new SettingsWindow();
            settings.ShowDialog();
        }

        //alow moving of PSP
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            try
            {
                // Begin dragging the window
                this.DragMove();
            }
            catch
            {

            }
        }


        #endregion << Events >>

        #region << Methods >>
        public static string AppCommonPath()
        {
            string returnstring = "";
            if (Properties.Settings.Default.OverwriteTemp == true && Properties.Settings.Default.TempPath != string.Empty)
            {
                returnstring = Properties.Settings.Default.TempPath + @"\Ps4Tools\";
            }
            else
            {
                returnstring = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Ps4Tools\";
            }
            return returnstring;
        }

        #region << Orbis >>
        public string Orbis_CMD(string command, string arguments)
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = AppCommonPath() + "pkg.exe " + command;
            start.Arguments = arguments;
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            start.CreateNoWindow = true;
            using (Process process = Process.Start(start))
            {
                process.ErrorDataReceived += Process_ErrorDataReceived;
                using (StreamReader reader = process.StandardOutput)
                {
                    string result = reader.ReadToEnd();
                    if (result.Contains("already converted from elf file to self file"))
                    {
                        System.Windows.Forms.DialogResult dlr = System.Windows.Forms.MessageBox.Show("Already Converted From Elf Error Found.... will be using Orbis-pub-gen for this pkg\n\n Simply Click Build and select the save folder", "Error with an alternative", System.Windows.Forms.MessageBoxButtons.OKCancel, System.Windows.Forms.MessageBoxIcon.Question);
                        if (dlr == System.Windows.Forms.DialogResult.OK)
                        {
                            //this will open up the GP4 Project inside the Utility
                            Orbis_Pub__GenCMD("", AppCommonPath() + @"\PSPEmu\" + "PS2Classics.gp4");

                        }
                    }
                    else if (result.Contains("[Error]"))
                    {
                        System.Windows.Forms.MessageBox.Show(result);
                    }
                    return result;
                }
            }
        }


        private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data.ToString().Trim() == "")
            {
                System.Windows.Forms.MessageBox.Show(e.Data.ToString());
            }
        }

        public string Orbis_Pub__GenCMD(string command, string arguments)
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = AppCommonPath() + "orbis-pub-gen.exe " + command;
            start.Arguments = arguments;
            start.UseShellExecute = false;
            start.RedirectStandardOutput = false;
            start.CreateNoWindow = false;
            using (Process process = Process.Start(start))
            {
                process.WaitForExit();
            }
            return "";
        }

        #endregion << Orbis >>


        //If you get 'dllimport unknown'-, then add 'using System.Runtime.InteropServices;'
        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject([In] IntPtr hObject);

        public ImageSource ImageSourceForBitmap(Bitmap bmp)
        {
            var handle = bmp.GetHbitmap();
            try
            {
                return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            finally { DeleteObject(handle); }
        }


        public ImageBrush ImageBrushFromBitmap(Bitmap bmp)
        {
            var bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bmp.GetHbitmap(),
                                  IntPtr.Zero,
                                  Int32Rect.Empty,
                                  BitmapSizeOptions.FromEmptyOptions());
            return new ImageBrush(bitmapSource);
        }


        Bitmap GetBitmap(BitmapSource source)
        {
            Bitmap bmp = new Bitmap(
              source.PixelWidth,
              source.PixelHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            System.Drawing.Imaging.BitmapData data = bmp.LockBits(
              new System.Drawing.Rectangle(System.Drawing.Point.Empty, bmp.Size),
              System.Drawing.Imaging.ImageLockMode.WriteOnly,
              System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            source.CopyPixels(
              Int32Rect.Empty,
              data.Scan0,
              data.Height * data.Stride,
              data.Stride);
            bmp.UnlockBits(data);
            return bmp;
        }

        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        public static Bitmap ResizeImage(System.Drawing.Image image, int width, int height)
        {
            var destRect = new System.Drawing.Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new System.Drawing.Imaging.ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }


        /// <summary>
        /// this convers images to 24bbp
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        public static Bitmap ConvertTo24bpp(Bitmap img)
        {
            var bmp = new Bitmap(img.Width, img.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            using (var gr = Graphics.FromImage(bmp))
                gr.DrawImage(img, new System.Drawing.Rectangle(0, 0, img.Width, img.Height));
            return bmp;
        }


        public static void DoEvents()
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background,
                                                  new Action(delegate { }));
        }



        public void LoadGp4()
        {
            //create new XML Document 
            xmldoc = new XmlDataDocument();
            //nodelist 
            XmlNodeList xmlnode;
            //setup the resource file to be extarcted
            string RunningPath = AppDomain.CurrentDomain.BaseDirectory;
            //load the xml file from the base directory
            xmldoc.Load(AppCommonPath() + @"\PSPEmu\" + "PS2Classics.gp4");
            //now load the nodes
            xmlnode = xmldoc.GetElementsByTagName("volume");//volume is inside the xml
            //loop to get all info from the node list
            foreach (XmlNode xn in xmlnode)
            {
                XmlNode xNode = xn.SelectSingleNode("package");
                if (xNode != null)
                {
                    //we found the info we are looking for
                    xmlcontentid = xNode.Attributes[0].Value.ToString();//fetch the attribute
                }
            }

        }

        /// <summary>
        /// Save the GP4 so we can build the PKG Via Command Prompt
        /// </summary>
        public void SaveGp4()
        {
            try
            {

                //create new XML Document 
                xmldoc = new XmlDataDocument();
                //nodelist 
                XmlNodeList xmlnode;
                //setup the resource file to be extarcted
                string RunningPath = AppDomain.CurrentDomain.BaseDirectory;
                //load the xml file from the base directory
                xmldoc.Load(AppCommonPath() + @"PSPEmu\" + "PSPClassics.gp4");
                //now load the nodes
                xmlnode = xmldoc.GetElementsByTagName("volume");//volume is inside the xml
                                                                //loop to get all info from the node list
                foreach (XmlNode xn in xmlnode)
                {
                    XmlNode xNode = xn.SelectSingleNode("package");
                    if (xNode != null)
                    {
                        //we found the info we are looking for
                        xNode.Attributes[0].Value = xmlcontentid;//set the attribute
                    }
                }
                ////Uncomment this if you want to use the current datetime
                //xmlnode = xmldoc.GetElementsByTagName("volume_ts");
                //foreach (XmlNode item in xmlnode)
                //{
                //    item.InnerText = DateTime.Now.ToString("YYYY-MM-DD HH:mm:ss");//2018-03-21 15:37:08
                //}

                xmldoc.Save(AppCommonPath() + @"\PSPEmu\" + "PSPClassics.gp4");

                //im cheating here a bit 

                //line builder
                //string tempval = @"    <file targ_path=""data/GAME.iso"" orig_path=""..\PSP\image\disc01.iso""" + @" />";
                //string builder = string.Empty;

                //for (int i = 0; i < MainWindow.pspfiles.Count; i++)
                //{
                //    builder += tempval.Replace("disc01.iso", "disc0" + (i + 1) + ".iso") + "\n";
                //}

                //var alllines = File.ReadAllText(AppCommonPath() + @"\PSPEmu\" + "PS2Classics.gp4");

                //alllines = alllines.Replace(tempval, builder.Remove(builder.Length - 1, 1));

                //File.WriteAllText(AppCommonPath() + @"\PSPEmu\" + "PS2Classics.gp4", alllines);

                #region << Configs >>
                //if (pspfiles.Count > 1)
                //{
                //    //line builder
                //    tempval = @"    <file targ_path=""patches/SLES-50366_cli.conf"" orig_path=""..\PS2\patches\SLES-50366_cli.conf""" + @" />";
                //    builder = string.Empty;

                //    for (int i = 0; i < MainWindow.PS2CutomLua.Count; i++)
                //    {
                //        builder += tempval.Replace("SLES-50366_cli.conf", PS2TitleId[i].ToString() /*Game Name Here*/+ "_cli.conf") + "\n";
                //    }

                //    alllines = File.ReadAllText(AppCommonPath() + @"\PSPEmu\" + "PS2Classics.gp4");

                //    alllines = alllines.Replace("@addps2patchHere", builder.Remove(builder.Length - 1, 1));

                //    File.WriteAllText(AppCommonPath() + @"\PSPEmu\" + "PS2Classics.gp4", alllines);


                //    tempval = @"    <file targ_path=""lua_include/SLUS-20071_config.lua"" orig_path=""..\PS2\lua_include\SLUS-20071_config.lua""" + @" />";
                //    builder = string.Empty;

                //    for (int i = 0; i < MainWindow.PS2CutomLua.Count; i++)
                //    {
                //        builder += tempval.Replace("SLUS-20071_config.lua", PS2TitleId[i].ToString().Replace(".","") /*Game Name Here*/+ "_config.lua") + "\n";
                //    }

                //    alllines = File.ReadAllText(AppCommonPath() + @"\PSPEmu\" + "PS2Classics.gp4");

                //    alllines = alllines.Replace("@addps2luhere", builder.Remove(builder.Length - 1, 1));

                //    File.WriteAllText(AppCommonPath() + @"\PSPEmu\" + "PS2Classics.gp4", alllines);

                //}
                //else
                //{
                //    alllines = alllines.Replace("@addps2patchHere", "");//remove the string
                //    alllines = alllines.Replace("@addps2luhere", "");//remove the string
                //    File.WriteAllText(AppCommonPath() + @"\PSPEmu\" + "PS2Classics.gp4", alllines);
                //}
                #endregion << Configs >>

            }
            catch (Exception ex)
            {
                var file = File.ReadAllText(AppCommonPath() + @"PSPEmu\" + "PSPClassics.gp4").Replace("UP9000-CUSA32644_00-NPUG801350000000", xmlcontentid);
                File.WriteAllText(AppCommonPath() + @"PSPEmu\" + "PSPClassics.gp4", file);
            }
        }

        public bool doesStringMatch()
        {
            if (pspfiles.Count == 0)
            {
                return false;
            }

            string txt = string.Empty;
            if (pspfiles.Count > 1)
            {
                txt = "UP9000-" + txtContentID.Text.Trim() + "_00-" + txtContentID.Text.Trim() + "0000001";//make this the same no ps2 id required

            }
            else
            {
                txt = "UP9000-" + txtContentID.Text.Trim() + "_00-" + PSPID.Replace("_", "") + "0000001";
            }

            string re1 = "((?:[a-z][a-z]*[0-9]+[a-z0-9]*))";    // Alphanum 1
            string re2 = "(-)"; // Any Single Character 1
            string re3 = "([a-z])"; // Any Single Word Character (Not Whitespace) 1
            string re4 = "([a-z])"; // Any Single Word Character (Not Whitespace) 2
            string re5 = "([a-z])"; // Any Single Word Character (Not Whitespace) 3
            string re6 = "([a-z])"; // Any Single Word Character (Not Whitespace) 4
            string re7 = "(\\d)";   // Any Single Digit 1
            string re8 = "(\\d)";   // Any Single Digit 2
            string re9 = "(\\d)";   // Any Single Digit 3
            string re10 = "(\\d)";  // Any Single Digit 4
            string re11 = "(\\d)";  // Any Single Digit 5
            string re12 = "(_)";    // Any Single Character 2
            string re13 = "(\\d+)"; // Integer Number 1
            string re14 = "(-)";    // Any Single Character 3
            string re15 = "((?:[a-z][a-z]*[0-9]+[a-z0-9]*))";   // Alphanum 2

            Regex r = new Regex(re1 + re2 + re3 + re4 + re5 + re6 + re7 + re8 + re9 + re10 + re11 + re12 + re13 + re14 + re15, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            Match m = r.Match(txt);
            if (m.Success)
            {
                String alphanum1 = m.Groups[1].ToString();
                String c1 = m.Groups[2].ToString();
                String w1 = m.Groups[3].ToString();
                String w2 = m.Groups[4].ToString();
                String w3 = m.Groups[5].ToString();
                String w4 = m.Groups[6].ToString();
                String d1 = m.Groups[7].ToString();
                String d2 = m.Groups[8].ToString();
                String d3 = m.Groups[9].ToString();
                String d4 = m.Groups[10].ToString();
                String d5 = m.Groups[11].ToString();
                String c2 = m.Groups[12].ToString();
                String int1 = m.Groups[13].ToString();
                String c3 = m.Groups[14].ToString();
                String alphanum2 = m.Groups[15].ToString();
                Console.Write("(" + alphanum1.ToString() + ")" + "(" + c1.ToString() + ")" + "(" + w1.ToString() + ")" + "(" + w2.ToString() + ")" + "(" + w3.ToString() + ")" + "(" + w4.ToString() + ")" + "(" + d1.ToString() + ")" + "(" + d2.ToString() + ")" + "(" + d3.ToString() + ")" + "(" + d4.ToString() + ")" + "(" + d5.ToString() + ")" + "(" + c2.ToString() + ")" + "(" + int1.ToString() + ")" + "(" + c3.ToString() + ")" + "(" + alphanum2.ToString() + ")" + "\n");
                return true;
            }
            //we need to make this work somehow
            return false;
        }

        public bool CheckString()
        {
            if (doesStringMatch() == false)
            {
                if (System.Windows.Forms.DialogResult.OK == System.Windows.Forms.MessageBox.Show("Content ID for this package is in the incorect format\n\nWould you like to edit this?", "Pakcage Content ID", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Error))
                {
                    //check the string
                    CheckString();
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        public void ExtractAllResources()
        {
            UpdateInfo("Checking Directory Paths");
            if (!Directory.Exists(AppCommonPath()))
            {
                UpdateInfo("Created Directory" + AppCommonPath());
                Directory.CreateDirectory(AppCommonPath());
            }
            if (!Directory.Exists(AppCommonPath() + @"\PSPEmu\"))
            {
                UpdateInfo("Created Directory" + AppCommonPath() + @"\PSPEmu\");
                Directory.CreateDirectory(AppCommonPath() + @"\PSPEmu\");
            }

            UpdateInfo("Writing All Binary Files to Temp Path....");

            //copy byte files
            System.IO.File.WriteAllBytes(AppCommonPath() + @"\PSPEmu\" + "PSPClassics.gp4", Properties.Resources.psphd1);
            UpdateInfo("Writing Binary File to Temp Path " + "\n Written : " + AppCommonPath() + @"\PSPEmu\" + "PSPClassics.gp4");
            System.IO.File.WriteAllBytes(AppCommonPath() + @"\PSPEmu\" + "param.sfo", Properties.Resources.param);
            UpdateInfo("Writing Binary File to Temp Path " + "\n Written : " + AppCommonPath() + @"\PSPEmu\" + "param.sfo");
            System.IO.File.WriteAllBytes(AppCommonPath() + "orbis-pub-cmd.exe", Properties.Resources.orbis_pub_cmd);
            UpdateInfo("Writing Binary File to Temp Path " + "\n Written : " + AppCommonPath() + "orbis-pub-cmd.exe");

            System.IO.File.WriteAllBytes(AppCommonPath() + "PSP.zip", Properties.Resources.psphd);
            UpdateInfo("Writing Binary File to Temp Path " + "\n Written : " + AppCommonPath() + "PSP.zip");
            System.IO.File.WriteAllBytes(AppCommonPath() + "ext.zip", Properties.Resources.ext);
            UpdateInfo("Writing Binary File to Temp Path " + "\n Written : " + AppCommonPath() + "ext.zip");


            System.IO.File.WriteAllBytes(AppCommonPath() + "psppkg.zip", Properties.Resources.psppkg);
            UpdateInfo("Writing Binary File to Temp Path " + "\n Written : " + AppCommonPath() + "psppkg.zip");

            UpdateInfo("Writing Image Files to Temp Path...");

            //copy images for the save process
            Properties.Resources.icon0.Save(AppCommonPath() + @"\PSPEmu\" + "icon0.png");
            UpdateInfo("Writing Image File to Temp Path " + "\n Written : " + AppCommonPath() + @"\PSPEmu\" + "icon0.png");
            Properties.Resources.icon0.Save(AppCommonPath() + @"\PSPEmu\" + "pic0.png");
            UpdateInfo("Writing Image File to Temp Path " + "\n Written : " + AppCommonPath() + @"\PSPEmu\" + "pic0.png");
            Properties.Resources.icon0.Save(AppCommonPath() + @"\PSPEmu\" + "pic1.png");
            UpdateInfo("Writing Image File to Temp Path " + "\n Written : " + AppCommonPath() + @"\PSPEmu\" + "pic1.png");

            UpdateInfo("Writing Text Files to Temp Path...");
            //copy text files
            System.IO.File.WriteAllText(AppCommonPath() + @"\PSPEmu\" + "sfo.xml", Properties.Resources.sfo);
            UpdateInfo("Writing Image File to Temp Path " + "\n Written : " + AppCommonPath() + @"\PSPEmu\" + "sfo.xml");


            UpdateInfo("Extracting Zip(s)");
            //extarct zip
            if (Directory.Exists(AppCommonPath() + @"\PSP\"))
            {
                DeleteDirectory(AppCommonPath() + @"\PSP\");
            }
            ZipFile.ExtractToDirectory(AppCommonPath() + "PSP.zip", AppCommonPath() + @"\PSP\");


            if (Directory.Exists(AppCommonPath() + @"\ext\"))
            {
                DeleteDirectory(AppCommonPath() + @"\ext\");
            }
            if (File.Exists(AppCommonPath() + @"\pkg.exe"))
            {
                File.Delete(AppCommonPath() + @"\pkg.exe");
            }
            //ZipFile.ExtractToDirectory(AppCommonPath() + "ext.zip", AppCommonPath());

            ZipFile.ExtractToDirectory(AppCommonPath() + "psppkg.zip", AppCommonPath());

            File.Delete(AppCommonPath() + "ext.zip");
            File.Delete((AppCommonPath() + "PSP.zip"));
            File.Delete((AppCommonPath() + "psppkg.zip"));
        }

        public static void DeleteDirectory(string target_dir)
        {
            string[] files = Directory.GetFiles(target_dir);
            string[] dirs = Directory.GetDirectories(target_dir);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }

            Directory.Delete(target_dir, false);
        }

        /// <summary>
        /// Converts Byte Array to Image
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public BitmapImage ToImage(byte[] array)
        {
            using (var ms = new System.IO.MemoryStream(array))
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad; // here
                image.StreamSource = ms;
                image.EndInit();
                return image;
            }
        }

        /// <summary>
        /// Converts stream to byte array
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }


        /// <summary>
        /// updates the advanced window
        /// </summary>
        /// <param name="info"></param>
        public void UpdateInfo(string info)
        {
            if (Properties.Settings.Default.EnableAdvancedMode == true)
            {
                advanced.Dispatcher.Invoke(new Action(() => advanced.LabelText += info + Environment.NewLine));
            }
        }

        private void UpdateString(string txt)
        {
            UpdateInfo(txt);
            OpenCloseWaitScreen(true);
            Busy.INFO = txt;
            //lblTask.Invoke(new Action(() => lblTask.Text = txt));
        }

        private void OpenCloseWaitScreen(bool open)
        {
            if (open)
            {
                //Info Screen Wait screen
                Busy.INFO = "Loading Data";
                bgwClose = false;
                if (!bgWorkerSS.IsBusy)
                {
                    bgWorkerSS.RunWorkerAsync();
                }
            }
            else
            {
                //Wait screen/ info Screen
                bgwClose = true;
                bgWorkerSS.CancelAsync();
                bgWorkerSS.Dispose();
            }
        }

        #endregion << Methods >>

       
    }
}
