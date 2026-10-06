// Profil görsellerini SVG olarak üretir (yalnızca saf #000 / #fff): node build.js
const fs = require('fs');
fs.mkdirSync('assets', { recursive: true });

const themes = {
  light: { bg: '#ffffff', fg: '#000000' },
  dark:  { bg: '#000000', fg: '#ffffff' },
};
const sans = `font-family="'Helvetica Neue','Inter','Segoe UI',Arial,sans-serif"`;
const esc = s => s.replace(/&/g, '&amp;').replace(/</g, '&lt;');

// çapraz tarama deseni (yalnızca saf çizgiler, gri ton yok)
const hatch = (id, t, gap = 10, w = 2) => `<pattern id="${id}" width="${gap}" height="${gap}" patternUnits="userSpaceOnUse" patternTransform="rotate(45)"><rect width="${gap}" height="${gap}" fill="${t.bg}"/><rect width="${w}" height="${gap}" fill="${t.fg}"/></pattern>`;
const arrow = (x, y, s, c) => `<path d="M${x} ${y} H${x + s} M${x + s - s * 0.35} ${y - s * 0.35} L${x + s} ${y} L${x + s - s * 0.35} ${y + s * 0.35}" fill="none" stroke="${c}" stroke-width="3" stroke-linecap="square"/>`;

function banner(t) {
  return `<svg xmlns="http://www.w3.org/2000/svg" width="1280" height="400" viewBox="0 0 1280 400" role="img" aria-label="ArnoldGods — AI systems, built in the open.">
  <defs>${hatch('h', t, 12, 2)}</defs>
  <rect width="1280" height="400" fill="${t.bg}"/>
  <rect x="2" y="2" width="1276" height="396" fill="none" stroke="${t.fg}" stroke-width="4"/>
  <g ${sans} font-size="13" font-weight="700" fill="${t.fg}" letter-spacing="2.5">
    <text x="40" y="40">PORTFOLIO — 2026</text>
    <text x="640" y="40" text-anchor="middle">AI SYSTEMS / AGENTS / TOOLING</text>
    <text x="1240" y="40" text-anchor="end">TR — OPEN SOURCE</text>
  </g>
  <rect x="2" y="60" width="1276" height="3" fill="${t.fg}"/>
  <text x="34" y="228" ${sans} font-size="172" font-weight="900" letter-spacing="-9" fill="${t.fg}" textLength="1212" lengthAdjust="spacingAndGlyphs">ArnoldGods</text>
  <rect x="2" y="262" width="1276" height="3" fill="${t.fg}"/>
  <rect x="2" y="265" width="880" height="133" fill="${t.fg}"/>
  <text x="40" y="326" ${sans} font-size="40" font-weight="800" fill="${t.bg}" letter-spacing="-1">AI systems,</text>
  <text x="40" y="372" ${sans} font-size="40" font-weight="800" fill="${t.bg}" letter-spacing="-1">built in the open.</text>
  <rect x="882" y="265" width="396" height="133" fill="url(#h)"/>
  <rect x="882" y="265" width="3" height="133" fill="${t.fg}"/>
  <rect x="930" y="300" width="300" height="64" fill="${t.bg}" stroke="${t.fg}" stroke-width="3"/>
  <text x="954" y="340" ${sans} font-size="17" font-weight="800" fill="${t.fg}" letter-spacing="3">BUILD. TEST. SHARE.</text>
</svg>`;
}

const cards = [
  { id: 'agent-hafiza', no: '01', tag: 'MEMORY', title: 'agent-hafiza', desc: ['Oturumlar arasında bağlamı koruyan,', 'Markdown tabanlı yerel ikinci beyin.'] },
  { id: 'claude-skills', no: '02', tag: 'WORKFLOWS', title: 'claude-skills', desc: ['Günlük işlerden çıkan, yeniden kullanılabilir', 'ajan becerileri: kod, review, araştırma.'] },
  { id: 'model-arena', no: '03', tag: 'EXPERIMENTS', title: 'model-arena', desc: ['Aynı prompt, farklı modeller. Orijinal', 'çıktılar ve maliyet açıkta.'] },
  { id: 'terminal-kit', no: '04', tag: 'TOOLS', title: 'terminal-kit', desc: ['Terminal ve çoklu-ajan iş akışları için', 'küçük, hızlı komut satırı araçları.'] },
];

function card(c, t) {
  return `<svg xmlns="http://www.w3.org/2000/svg" width="620" height="260" viewBox="0 0 620 260" role="img" aria-label="${esc(c.title)} — ${esc(c.desc.join(' '))}">
  <defs>${hatch('h', t, 9, 2)}</defs>
  <rect width="620" height="260" fill="${t.bg}"/>
  <rect x="2" y="2" width="616" height="256" fill="none" stroke="${t.fg}" stroke-width="4"/>
  <text x="26" y="42" ${sans} font-size="13" font-weight="800" letter-spacing="3" fill="${t.fg}">${c.tag}</text>
  <rect x="26" y="52" width="40" height="4" fill="${t.fg}"/>
  <text x="594" y="118" text-anchor="end" ${sans} font-size="120" font-weight="900" letter-spacing="-6" fill="${t.bg}" stroke="${t.fg}" stroke-width="2.5">${c.no}</text>
  <text x="24" y="130" ${sans} font-size="42" font-weight="900" letter-spacing="-2" fill="${t.fg}">${esc(c.title)}</text>
  <text x="26" y="162" ${sans} font-size="15" font-weight="500" fill="${t.fg}">${esc(c.desc[0])}</text>
  <text x="26" y="184" ${sans} font-size="15" font-weight="500" fill="${t.fg}">${esc(c.desc[1])}</text>
  <rect x="2" y="204" width="616" height="54" fill="${t.fg}"/>
  <text x="26" y="238" ${sans} font-size="15" font-weight="800" letter-spacing="3" fill="${t.bg}">DEPOYU AÇ</text>
  <rect x="470" y="204" width="148" height="54" fill="url(#h)"/>
  <rect x="470" y="204" width="3" height="54" fill="${t.bg}"/>
  ${arrow(404, 231, 40, t.bg)}
</svg>`;
}

const stack = ['CLAUDE CODE', 'NODE.JS', 'TYPESCRIPT', 'PYTHON', 'GIT', 'OBSIDIAN'];
function stackStrip(t) {
  let x = 2, out = '';
  for (let i = 0; i < stack.length; i++) {
    const w = stack[i].length * 10.5 + 44;
    const inv = i % 2 === 1;
    out += `<rect x="${x}" y="2" width="${w}" height="52" fill="${inv ? t.fg : t.bg}" stroke="${t.fg}" stroke-width="3"/>`;
    out += `<text x="${x + w / 2}" y="34" text-anchor="middle" ${sans} font-size="14" font-weight="800" letter-spacing="2" fill="${inv ? t.bg : t.fg}">${stack[i]}</text>`;
    x += w - 3;
  }
  return `<svg xmlns="http://www.w3.org/2000/svg" width="${Math.ceil(x + 5)}" height="56" viewBox="0 0 ${Math.ceil(x + 5)} 56" role="img" aria-label="Araç çantası: ${stack.join(', ')}">${out}</svg>`;
}


for (const [name, t] of Object.entries(themes)) { // banner/kartlar agy ile PNG üretiliyor (tools/); burada yalnızca stack
  fs.writeFileSync(`assets/stack-${name}.svg`, stackStrip(t)); continue;
  fs.writeFileSync(`assets/banner-${name}.svg`, banner(t));
  fs.writeFileSync(`assets/stack-${name}.svg`, stackStrip(t));
  for (const c of cards) fs.writeFileSync(`assets/card-${c.id}-${name}.svg`, card(c, t));
}
console.log('ok');
