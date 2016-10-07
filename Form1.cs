using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


// ビットマップを生成するため
using System.Drawing.Imaging;

// Kinectのセンサクラス
// Microsoft.Kinectを参照に追加
using Microsoft.Kinect;

// メモリ管理の最適化のため
// System.Runtime.Serializationを参照に追加
using System.Runtime.InteropServices;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {

        /// <summary>
        /// kinectのセンサクラス
        /// </summary>
        KinectSensor kinect;

        /// <summary>
        /// kinectから取得したRGBデータ
        /// （byte型配列）
        /// </summary>
        byte[] imageData;

        /// <summary>
        /// 実画像
        /// （ビットマップ形式）
        /// </summary>
        Bitmap ImageBitmap;

        public Form1()
        {
            InitializeComponent();

            // ピットマップの初期化
            ImageBitmap = new Bitmap(640,480);

            // kinectの初期化
            kinect = KinectSensor.KinectSensors[0];

            // カラー画像の取得を開始する
            kinect.ColorFrameReady += new EventHandler<ColorImageFrameReadyEventArgs>(ColorImageReady);
            kinect.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);

            // Kinectを起動する
            kinect.Start();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // 画像の保存
            ImageBitmap.Save(DateTime.Now.Year + " - " + DateTime.Now.Month + "-" + DateTime.Now.Day + "-" + DateTime.Now.Hour + "-" + DateTime.Now.Minute + "-" + DateTime.Now.Second + ".bmp");
        }

        /// <summary>
        /// カラー画像の取得
        /// </summary>


        void ColorImageReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            // kinectからカラーイメージを取得
            ColorImageFrame image = e.OpenColorImageFrame();

            // imageがnullだった場合処理しない
            if (image != null)
            {
                // imageData配列の初期化
                imageData = new byte[image.PixelDataLength];

                // imageのピクセルデータをpixelDataへコピーする
                image.CopyPixelDataTo(imageData);

                // imageDataからビットマップへ変換する
                ImageBitmap = toBitmap(imageData, ImageBitmap.Width, ImageBitmap.Height);

                // ピクチャーボックスへ反映
                pictureBox1.Image = ImageBitmap;

            }
        }

        /// 取得データをビットマップデータに変換
        /// </summary>
        /// <param name="pixels">kinectで取得したbyte[]配列</param>
        /// <param name="width">横サイズ</param>
        /// <param name="height">縦サイズ</param>
        /// <returns></returns>
        public static Bitmap toBitmap(byte[] pixels, int width, int height)
        {
            // pixelsに何も入っていない場合nullを返す
            if (pixels == null)
                return null;

            // ビットマップの初期化
            var bitmap = new Bitmap(width, height, PixelFormat.Format32bppRgb);

            // システムメモリへロック
            var data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);

            // メモリデータのコピー
            Marshal.Copy(pixels, 0, data.Scan0, pixels.Length);

            // システムメモリのロック解除
            bitmap.UnlockBits(data);

            return bitmap;
        }

      
    }
}
