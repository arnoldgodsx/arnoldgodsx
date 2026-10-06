// profile.html içindeki her <section>ı açık/koyu PNG olarak çıkarır. Karakter görselleri POSES klasöründen okunur.
// Kullanım: npm i playwright-core && npx playwright install chromium && node tools/render.js assets
const { chromium } = require('playwright-core');
const fs = require('fs');
const path = require('path');

const POSES = (process.env.POSES || "C:/Users/ArnoldGods/Desktop/karakter-pozlar") + "/";
const imgs = JSON.parse(fs.readFileSync(path.join(__dirname, 'images.json'), 'utf8'));
const out = process.argv[2];
fs.mkdirSync(out, { recursive: true });

let html = fs.readFileSync(path.join(__dirname, 'profile.html'), 'utf8');
for (const [k, v] of Object.entries(imgs)) html = html.replaceAll(k, 'file:///' + POSES + v);
const tmp = path.join(__dirname, '_built.html');
fs.writeFileSync(tmp, html);

const names = { hero: 'hero', 'h-work': 'head-work', 'card-apeks': 'card-apeks', 'card-dw': 'card-arnolddw',
  'card-corridor': 'card-corridor', 'card-shipnote': 'card-shipnote', 'h-yt': 'head-youtube', yt: 'youtube',
  'h-about': 'head-about', about: 'about', foot: 'footer' };

(async () => {
  const b = await chromium.launch();
  const p = await b.newPage({ viewport: { width: 1280, height: 900 }, deviceScaleFactor: 2 });
  await p.goto('file:///' + tmp.replace(/\\/g, '/'), { waitUntil: 'networkidle' });
  await p.evaluate(() => document.fonts.ready);
  for (const theme of ['light', 'dark']) {
    await p.evaluate(t => document.documentElement.classList.toggle('dark', t === 'dark'), theme);
    for (const [id, n] of Object.entries(names)) {
      await p.locator('#' + id).screenshot({ path: path.join(out, `${n}-${theme}.png`) });
    }
  }
  await b.close();
  console.log('ok');
})();
