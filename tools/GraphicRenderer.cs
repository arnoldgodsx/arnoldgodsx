using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

public class GraphicRenderer {

    private static readonly string[] ForbiddenWords = new string[] {
        "awwwards", "binary", "brutalism", "spec", "status", "resolution"
    };

    public static void AssertNoForbiddenWords(params string[] texts) {
        foreach (string text in texts) {
            if (string.IsNullOrEmpty(text)) continue;
            foreach (string forbidden in ForbiddenWords) {
                if (Regex.IsMatch(text, @"\b" + Regex.Escape(forbidden) + @"\b", RegexOptions.IgnoreCase)) {
                    throw new InvalidOperationException("Forbidden word detected: '" + forbidden + "' in text: '" + text + "'");
                }
            }
        }
    }

    public static void BinarizeBitmap(Bitmap bmp) {
        int width = bmp.Width;
        int height = bmp.Height;
        BitmapData data = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
        int bytes = Math.Abs(data.Stride) * height;
        byte[] rgb = new byte[bytes];
        Marshal.Copy(data.Scan0, rgb, 0, bytes);

        for (int i = 0; i < bytes; i += 4) {
            int lum = (rgb[i] + rgb[i + 1] + rgb[i + 2]) / 3;
            byte val = lum > 127 ? (byte)255 : (byte)0;
            rgb[i] = val;
            rgb[i + 1] = val;
            rgb[i + 2] = val;
            rgb[i + 3] = 255;
        }

        Marshal.Copy(rgb, 0, data.Scan0, bytes);
        bmp.UnlockBits(data);
    }

    public static void GenerateBanner(string outputPath, bool dark) {
        int width = 1280;
        int height = 400;

        string headerTag = "[ SYS // 01 ]";
        string headerMid1 = "INDEX: AUTONOMOUS COMPUTATION";
        string headerMid2 = "SYSTEM PROTOCOLS // MONOCHROME";
        string headerRight = "DIM: 1280x400  |  HARD-EDGE GRID";

        string catTag = "01 / CORE INFRASTRUCTURE";
        string wordmark = "ArnoldGods";
        string caption = "AI SYSTEMS, BUILT IN THE OPEN";
        string subCaption = "// HIGH-THROUGHPUT AGENTIC PROTOCOLS \u2022 COGNITIVE WORKFLOW ENGINES";

        string hatchLabelTop = "[ FIG. 01 \u2014 HATCH PATTERN BLOCK ]";
        string hatchLabelBot = "INDEX: GEOMETRIC / 45\u00b0 DENSITY";

        string footerLeft = "GITHUB.COM/ARNOLDGODSX";
        string footerMid = "AI SYSTEMS LABORATORY  //  OPEN SOURCE RESEARCH";
        string footerRight = "STATE: ACTIVE // PRODUCTION";

        AssertNoForbiddenWords(
            headerTag, headerMid1, headerMid2, headerRight,
            catTag, wordmark, caption, subCaption,
            hatchLabelTop, hatchLabelBot,
            footerLeft, footerMid, footerRight
        );

        using (Bitmap bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb)) {
            using (Graphics g = Graphics.FromImage(bmp)) {
                Color bg = dark ? Color.Black : Color.White;
                Color fg = dark ? Color.White : Color.Black;

                g.Clear(bg);
                g.SmoothingMode = SmoothingMode.None;
                g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;

                using (Pen pen1 = new Pen(fg, 1))
                using (Brush brush = new SolidBrush(fg))
                using (Font fontHuge = new Font("Arial Black", 84, FontStyle.Bold))
                using (Font fontCap = new Font("Segoe UI", 12, FontStyle.Bold))
                using (Font fontMonoBold = new Font("Segoe UI", 9, FontStyle.Bold))
                using (Font fontMono = new Font("Consolas", 9, FontStyle.Regular))
                using (Font fontMicro = new Font("Consolas", 8, FontStyle.Regular)) {

                    int m = 22; // outer margin
                    int innerW = width - 2 * m;
                    int innerH = height - 2 * m;

                    // Outer border
                    g.DrawRectangle(pen1, m, m, innerW, innerH);

                    // Header rule
                    int headerH = 34;
                    int topY = m + headerH;
                    g.DrawLine(pen1, m, topY, width - m, topY);

                    // Footer rule
                    int footerH = 34;
                    int botY = height - m - footerH;
                    g.DrawLine(pen1, m, botY, width - m, botY);

                    // Header metadata
                    g.DrawString(headerTag, fontMonoBold, brush, m + 14, m + 8);
                    g.DrawString(headerMid1, fontMono, brush, m + 160, m + 9);
                    g.DrawString(headerMid2, fontMono, brush, m + 450, m + 9);
                    SizeF szRight = g.MeasureString(headerRight, fontMono);
                    g.DrawString(headerRight, fontMono, brush, width - m - 14 - szRight.Width, m + 9);

                    // Footer metadata
                    g.DrawString(footerLeft, fontMonoBold, brush, m + 14, botY + 8);
                    g.DrawString(footerMid, fontMono, brush, m + 260, botY + 9);
                    SizeF szStatus = g.MeasureString(footerRight, fontMonoBold);
                    g.DrawString(footerRight, fontMonoBold, brush, width - m - 14 - szStatus.Width, botY + 8);

                    // Right column split (for Diagonal Hatch Pattern Block)
                    int rightBlockW = 340;
                    int splitX = width - m - rightBlockW;
                    g.DrawLine(pen1, splitX, topY, splitX, botY);

                    // --- LEFT MAIN AREA ---
                    int leftPad = 36;
                    int contentX = m + leftPad;

                    // Architecture tag above wordmark
                    g.DrawString(catTag, fontMicro, brush, contentX, topY + 22);

                    // Centerpiece: Huge bold sans-serif wordmark 'ArnoldGods'
                    int wordmarkY = topY + 40;
                    g.DrawString(wordmark, fontHuge, brush, contentX - 6, wordmarkY);

                    // Thin rule under wordmark (clear of descenders)
                    int ruleY = topY + 185;
                    g.DrawLine(pen1, contentX, ruleY, splitX - 36, ruleY);

                    // Small uppercase caption: 'AI SYSTEMS, BUILT IN THE OPEN'
                    int capY = ruleY + 14;
                    float curCapX = contentX;
                    for (int i = 0; i < caption.Length; i++) {
                        char c = caption[i];
                        if (c == ' ') {
                            curCapX += 16; // letterspacing gap
                        } else {
                            string s = c.ToString();
                            g.DrawString(s, fontCap, brush, curCapX, capY);
                            SizeF sz = g.MeasureString(s, fontCap);
                            curCapX += sz.Width + 2;
                        }
                    }

                    // Secondary descriptor
                    g.DrawString(subCaption, fontMicro, brush, contentX, capY + 28);

                    // --- RIGHT AREA: Diagonal hatch pattern block ---
                    int hPadX = 22;
                    int hPadY = 28;
                    int hatchX = splitX + hPadX;
                    int hatchY = topY + hPadY;
                    int hatchW = rightBlockW - 2 * hPadX;
                    int hatchH = (botY - topY) - 2 * hPadY;

                    // Label above hatch block
                    g.DrawString(hatchLabelTop, fontMicro, brush, hatchX, hatchY - 16);

                    // Frame the hatch block
                    g.DrawRectangle(pen1, hatchX, hatchY, hatchW, hatchH);

                    // Fill with uninterrupted 45-degree diagonal lines
                    Region oldClip = g.Clip;
                    g.SetClip(new Rectangle(hatchX + 1, hatchY + 1, hatchW - 1, hatchH - 1));

                    int hatchSpacing = 8;
                    for (int d = -hatchH - 100; d < hatchW + hatchH + 100; d += hatchSpacing) {
                        g.DrawLine(pen1, hatchX + d, hatchY, hatchX + d - hatchH, hatchY + hatchH);
                    }
                    g.Clip = oldClip;

                    // Label below hatch block
                    g.DrawString(hatchLabelBot, fontMicro, brush, hatchX, hatchY + hatchH + 6);

                    // Registration crosshairs
                    Action<int, int> drawCross = (cx, cy) => {
                        int s = 5;
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
                    drawCross(m, topY + (botY - topY) / 2);
                    drawCross(width - m, topY + (botY - topY) / 2);
                }
            }

            BinarizeBitmap(bmp);
            bmp.Save(outputPath, ImageFormat.Png);
        }
    }

    public static void GenerateProjectCard(
        string outputPath,
        bool dark,
        string indexNum,
        string categoryTag,
        string title,
        string description,
        string stackTag
    ) {
        int width = 1240;
        int height = 520;

        string headerMid = "AI SYSTEMS LABORATORY // LOCAL COGNITION";
        string headerRight = "INDEX // " + indexNum;
        string hatchLabelTop = "[ FIG. " + indexNum + " \u2014 HATCH STRIP ]";
        string hatchLabelBot = "INDEX: " + indexNum + " // 45\u00b0 DENSITY";
        string subModule = "MODULE ID: " + indexNum + " // OPEN ARCHITECTURE";
        string btnText = "DEPOYU A\u00c7 \u2192";
        string repoUrl = "GITHUB.COM/ARNOLDGODSX/" + title;

        AssertNoForbiddenWords(
            indexNum, categoryTag, title, description, stackTag,
            headerMid, headerRight, hatchLabelTop, hatchLabelBot,
            subModule, btnText, repoUrl
        );

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
                    g.DrawString(headerMid, fontMono, fgBrush, m + 250, m + 13);
                    SizeF szTr = g.MeasureString(headerRight, fontMono);
                    g.DrawString(headerRight, fontMono, fgBrush, width - m - 24 - szTr.Width, m + 13);

                    // Partition between left content and right graphic column
                    int rightW = 340;
                    int splitX = width - m - rightW;

                    // Bottom inverted bar
                    int botH = 54;
                    int botY = height - m - botH;

                    // Vertical partition line
                    g.DrawLine(pen1, splitX, topY, splitX, botY);

                    // Inverted bottom bar: full width across card bottom
                    Rectangle botBar = new Rectangle(m, botY, cardW, botH);
                    g.FillRectangle(invBgBrush, botBar);
                    g.DrawRectangle(pen1, botBar);

                    // Inverted button text and repo URL
                    g.DrawString(btnText, fontBtn, invFgBrush, m + 32, botY + 14);
                    SizeF szUrl = g.MeasureString(repoUrl, fontMono);
                    g.DrawString(repoUrl, fontMono, invFgBrush, width - m - 32 - szUrl.Width, botY + 18);

                    // --- RIGHT COLUMN: Hatch strips + Outlined large index number ---
                    int hatchH = 46;
                    int hatchY = topY + 24;
                    int hatchX = splitX + 24;
                    int hatchW = rightW - 48;

                    // Top hatch strip
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

                    // Bottom hatch strip
                    int hatch2Y = numY + numH + 16;
                    int hatch2H = 46;
                    g.DrawRectangle(pen1, hatchX, hatch2Y, hatchW, hatch2H);
                    g.SetClip(new Rectangle(hatchX + 1, hatch2Y + 1, hatchW - 1, hatch2H - 1));
                    for (int d = -hatch2H - 100; d < hatchW + hatch2H + 100; d += 6) {
                        g.DrawLine(pen1, hatchX + d, hatch2Y, hatchX + d - hatch2H, hatch2Y + hatch2H);
                    }
                    g.Clip = oldClip;

                    // Caption below bottom hatch strip
                    g.DrawString(hatchLabelBot, fontMicro, fgBrush, hatchX, hatch2Y + hatch2H + 10);

                    // --- LEFT MAIN AREA ---
                    int leftPad = 36;
                    int contentX = m + leftPad;
                    int contentW = splitX - contentX - 24;

                    // Module ID label above title
                    g.DrawString(subModule, fontMicro, fgBrush, contentX, topY + 22);

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

                    // Tech stack tag
                    int extraY = descY + 84;
                    g.DrawString(stackTag, fontMono, fgBrush, contentX, extraY);

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

            BinarizeBitmap(bmp);
            bmp.Save(outputPath, ImageFormat.Png);
        }
    }

    public static List<string> GenerateAll(string outputDirectory) {
        List<string> generatedFiles = new List<string>();

        // 1. Banners (1280x400)
        string bannerLight = Path.Combine(outputDirectory, "banner-light.png");
        string bannerDark = Path.Combine(outputDirectory, "banner-dark.png");
        GenerateBanner(bannerLight, false);
        GenerateBanner(bannerDark, true);
        generatedFiles.Add(bannerLight);
        generatedFiles.Add(bannerDark);

        // 2. Project Cards (1240x520)
        // Card 1: agent-hafiza
        string c1Light = Path.Combine(outputDirectory, "card-agent-hafiza-light.png");
        string c1Dark = Path.Combine(outputDirectory, "card-agent-hafiza-dark.png");
        string c1Desc = "Oturumlar aras\u0131nda ba\u011flam\u0131 koruyan, Markdown tabanl\u0131 yerel ikinci beyin.";
        string c1Stack = "// STACK: LOCAL MARKDOWN STORAGE \u2022 CONTEXT PERSISTENCE";
        GenerateProjectCard(c1Light, false, "01", "01 / MEMORY", "agent-hafiza", c1Desc, c1Stack);
        GenerateProjectCard(c1Dark, true, "01", "01 / MEMORY", "agent-hafiza", c1Desc, c1Stack);
        generatedFiles.Add(c1Light);
        generatedFiles.Add(c1Dark);

        // Card 2: claude-skills
        string c2Light = Path.Combine(outputDirectory, "card-claude-skills-light.png");
        string c2Dark = Path.Combine(outputDirectory, "card-claude-skills-dark.png");
        string c2Desc = "G\u00fcnl\u00fck i\u015flerden \u00e7\u0131kan, yeniden kullan\u0131labilir ajan becerileri.";
        string c2Stack = "// STACK: REUSABLE AGENT SKILLS \u2022 AUTOMATION PATTERNS";
        GenerateProjectCard(c2Light, false, "02", "02 / WORKFLOWS", "claude-skills", c2Desc, c2Stack);
        GenerateProjectCard(c2Dark, true, "02", "02 / WORKFLOWS", "claude-skills", c2Desc, c2Stack);
        generatedFiles.Add(c2Light);
        generatedFiles.Add(c2Dark);

        // Card 3: model-arena
        string c3Light = Path.Combine(outputDirectory, "card-model-arena-light.png");
        string c3Dark = Path.Combine(outputDirectory, "card-model-arena-dark.png");
        string c3Desc = "Ayn\u0131 prompt, farkl\u0131 modeller. Orijinal \u00e7\u0131kt\u0131lar ve maliyet a\u00e7\u0131kta.";
        string c3Stack = "// STACK: MULTI-MODEL BENCHMARK \u2022 TELEMETRY & COST LEDGER";
        GenerateProjectCard(c3Light, false, "03", "03 / EXPERIMENTS", "model-arena", c3Desc, c3Stack);
        GenerateProjectCard(c3Dark, true, "03", "03 / EXPERIMENTS", "model-arena", c3Desc, c3Stack);
        generatedFiles.Add(c3Light);
        generatedFiles.Add(c3Dark);

        // Card 4: terminal-kit
        string c4Light = Path.Combine(outputDirectory, "card-terminal-kit-light.png");
        string c4Dark = Path.Combine(outputDirectory, "card-terminal-kit-dark.png");
        string c4Desc = "Terminal ve \u00e7oklu-ajan i\u015f ak\u0131\u015flar\u0131 i\u00e7in k\u00fc\u00e7\u00fck komut sat\u0131r\u0131 ara\u00e7lar\u0131.";
        string c4Stack = "// STACK: CLI UTILITIES \u2022 MULTI-AGENT ORCHESTRATION";
        GenerateProjectCard(c4Light, false, "04", "04 / TOOLS", "terminal-kit", c4Desc, c4Stack);
        GenerateProjectCard(c4Dark, true, "04", "04 / TOOLS", "terminal-kit", c4Desc, c4Stack);
        generatedFiles.Add(c4Light);
        generatedFiles.Add(c4Dark);

        return generatedFiles;
    }

    public static void VerifyFile(string path, int expectedW, int expectedH) {
        if (!File.Exists(path)) throw new FileNotFoundException("Missing file: " + path);
        using (Bitmap bmp = new Bitmap(path)) {
            if (bmp.Width != expectedW || bmp.Height != expectedH) {
                throw new Exception(string.Format("Invalid dimensions for {0}: expected {1}x{2}, got {3}x{4}",
                    path, expectedW, expectedH, bmp.Width, bmp.Height));
            }

            BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            int bytes = Math.Abs(data.Stride) * bmp.Height;
            byte[] rgb = new byte[bytes];
            Marshal.Copy(data.Scan0, rgb, 0, bytes);
            bmp.UnlockBits(data);

            int blackCount = 0;
            int whiteCount = 0;
            int invalidCount = 0;

            for (int i = 0; i < bytes; i += 4) {
                byte b = rgb[i];
                byte g = rgb[i + 1];
                byte r = rgb[i + 2];
                byte a = rgb[i + 3];

                if (a == 255 && r == 0 && g == 0 && b == 0) {
                    blackCount++;
                } else if (a == 255 && r == 255 && g == 255 && b == 255) {
                    whiteCount++;
                } else {
                    invalidCount++;
                }
            }

            if (invalidCount > 0) {
                throw new Exception(string.Format("File {0} contains {1} non-binary pixels!", path, invalidCount));
            }

            Console.WriteLine(string.Format("VERIFIED: {0} ({1}x{2}) -> Pure Black: {3}, Pure White: {4}, Invalid: 0",
                Path.GetFileName(path), bmp.Width, bmp.Height, blackCount, whiteCount));
        }
    }
}
