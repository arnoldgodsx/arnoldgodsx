using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

public class CardGenerator {
    public static void GenerateCard(
        string outputPath,
        bool dark,
        string indexNum,
        string categoryTag,
        string title,
        string description,
        string techStack
    ) {
        int width = 1240;
        int height = 520;

        using (Bitmap bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb)) {
            using (Graphics g = Graphics.FromImage(bmp)) {
                Color bg = dark ? Color.Black : Color.White;
                Color fg = dark ? Color.White : Color.Black;
                Color invBg = dark ? Color.White : Color.Black;
                Color invFg = dark ? Color.Black : Color.White;

                g.Clear(bg);
                g.SmoothingMode = SmoothingMode.None;
                g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;

                using (Pen pen1 = new Pen(fg, 1))
                using (Pen pen2 = new Pen(fg, 2))
                using (Pen penThick = new Pen(fg, 3))
                using (Brush fgBrush = new SolidBrush(fg))
                using (Brush invBgBrush = new SolidBrush(invBg))
                using (Brush invFgBrush = new SolidBrush(invFg))
                using (Font fontBigNum = new Font("Arial Black", 125, FontStyle.Bold))
                using (Font fontTitle = new Font("Arial Black", 46, FontStyle.Bold))
                using (Font fontDesc = new Font("Segoe UI", 16, FontStyle.Regular))
                using (Font fontMetaBold = new Font("Segoe UI", 11, FontStyle.Bold))
                using (Font fontMono = new Font("Consolas", 10, FontStyle.Regular))
                using (Font fontMicro = new Font("Consolas", 8.5f, FontStyle.Regular))
                using (Font fontBtn = new Font("Segoe UI", 15, FontStyle.Bold)) {

                    int m = 20; // outer margin
                    int cardW = width - 2 * m;
                    int cardH = height - 2 * m;

                    // Thick outer border
                    g.DrawRectangle(penThick, m, m, cardW, cardH);

                    // Top header line
                    int topY = m + 42;
                    g.DrawLine(pen1, m, topY, width - m, topY);

                    // Top header metadata
                    g.DrawString(categoryTag, fontMetaBold, fgBrush, m + 24, m + 11);
                    g.DrawString("AI SYSTEMS LABORATORY // LOCAL COGNITION", fontMono, fgBrush, m + 250, m + 13);
                    string tagRight = "INDEX // " + indexNum;
                    SizeF szTr = g.MeasureString(tagRight, fontMono);
                    g.DrawString(tagRight, fontMono, fgBrush, width - m - 24 - szTr.Width, m + 13);

                    // Partition between left content and right graphic column
                    int rightW = 340;
                    int splitX = width - m - rightW;

                    // Bottom inverted bar
                    int botH = 54;
                    int botY = height - m - botH;

                    // Vertical partition line from header down to bottom bar
                    g.DrawLine(pen1, splitX, topY, splitX, botY);

                    // Full-width inverted black bar at bottom (white in dark mode)
                    Rectangle botBar = new Rectangle(m, botY, cardW, botH);
                    g.FillRectangle(invBgBrush, botBar);
                    g.DrawRectangle(pen1, botBar);

                    // Inverted text inside bottom bar: 'DEPOYU AÇ →'
                    g.DrawString("DEPOYU AÇ →", fontBtn, invFgBrush, m + 32, botY + 14);
                    string repoUrl = "GITHUB.COM/ARNOLDGODSX/" + title;
                    SizeF szUrl = g.MeasureString(repoUrl, fontMono);
                    g.DrawString(repoUrl, fontMono, invFgBrush, width - m - 32 - szUrl.Width, botY + 18);

                    // --- RIGHT COLUMN: Hatch strip + Outlined large index number ---
                    int hatchH = 50;
                    int hatchY = topY + 24;
                    int hatchX = splitX + 24;
                    int hatchW = rightW - 48;

                    // Hatch strip frame
                    g.DrawRectangle(pen1, hatchX, hatchY, hatchW, hatchH);
                    Region oldClip = g.Clip;
                    g.SetClip(new Rectangle(hatchX + 1, hatchY + 1, hatchW - 1, hatchH - 1));
                    for (int d = -hatchH - 100; d < hatchW + hatchH + 100; d += 6) {
                        g.DrawLine(pen1, hatchX + d, hatchY, hatchX + d - hatchH, hatchY + hatchH);
                    }
                    g.Clip = oldClip;

                    // Outlined large index number
                    int numY = hatchY + hatchH + 16;
                    int numH = 185;
                    using (GraphicsPath numPath = new GraphicsPath()) {
                        StringFormat sf = new StringFormat();
                        sf.Alignment = StringAlignment.Center;
                        sf.LineAlignment = StringAlignment.Center;
                        Rectangle numRect = new Rectangle(splitX, numY, rightW, numH);
                        numPath.AddString(indexNum, fontBigNum.FontFamily, (int)FontStyle.Bold, 125, numRect, sf);
                        g.DrawPath(pen2, numPath);
                    }

                    // Metadata caption below index number
                    string rFooter = "INDEX: " + indexNum + " // 45° DENSITY";
                    g.DrawString(rFooter, fontMicro, fgBrush, hatchX, numY + numH + 12);

                    // --- LEFT MAIN AREA ---
                    int leftPad = 36;
                    int contentX = m + leftPad;
                    int contentW = splitX - contentX - 24;

                    // Architecture label above title
                    g.DrawString("MODULE ID: " + indexNum + " // OPEN ARCHITECTURE", fontMicro, fgBrush, contentX, topY + 22);

                    // Big title
                    int titleY = topY + 42;
                    g.DrawString(title, fontTitle, fgBrush, contentX - 3, titleY);

                    // 1px rule under title
                    int ruleY = titleY + 80;
                    g.DrawLine(pen1, contentX, ruleY, splitX - 36, ruleY);

                    // Description text (proper Turkish characters)
                    int descY = ruleY + 24;
                    RectangleF descRect = new RectangleF(contentX, descY, contentW, 90);
                    g.DrawString(description, fontDesc, fgBrush, descRect);

                    // Tech stack metadata tags
                    int extraY = descY + 84;
                    g.DrawString(techStack, fontMono, fgBrush, contentX, extraY);

                    // Editorial registration crosshairs
                    Action<int, int> drawCross = (cx, cy) => {
                        int s = 6;
                        g.DrawLine(pen1, cx - s, cy, cx + s, cy);
                        g.DrawLine(pen1, cx, cy - s, cx, cy + s);
                    };

                    drawCross(m, m);
                    drawCross(width - m, m);
                    drawCross(m, height - m);
                    drawCross(width - m, height - m);
                    drawCross(m, topY);
                    drawCross(width - m, topY);
                    drawCross(m, botY);
                    drawCross(width - m, botY);
                    drawCross(splitX, topY);
                    drawCross(splitX, botY);
                }
            }

            // Strictly 1-bit thresholding
            BitmapData data = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            int bytes = Math.Abs(data.Stride) * height;
            byte[] rgbValues = new byte[bytes];
            Marshal.Copy(data.Scan0, rgbValues, 0, bytes);

            for (int i = 0; i < bytes; i += 4) {
                int lum = (rgbValues[i] + rgbValues[i + 1] + rgbValues[i + 2]) / 3;
                byte val = lum > 127 ? (byte)255 : (byte)0;
                rgbValues[i] = val;
                rgbValues[i + 1] = val;
                rgbValues[i + 2] = val;
                rgbValues[i + 3] = 255;
            }

            Marshal.Copy(rgbValues, 0, data.Scan0, bytes);
            bmp.UnlockBits(data);

            bmp.Save(outputPath, ImageFormat.Png);
        }
    }
}
