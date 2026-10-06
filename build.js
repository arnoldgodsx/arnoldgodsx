// Profil görsellerini (banner + proje kartları) SVG olarak üretir: node build.js
const fs = require('fs');
fs.mkdirSync('assets', { recursive: true });

const themes = {
  light: { bg: '#faf7f2', fg: '#1f2328', mute: '#656d76', accent: '#b45309', line: '#e3dccf', card: '#fffdf9' },
  dark:  { bg: '#0d1117', fg: '#e6edf3', mute: '#8b949e', accent: '#f59e0b', line: '#30363d', card: '#161b22' },
};
const font = "font-family=\"'Segoe UI',-apple-system,Helvetica,Arial,sans-serif\"";
const mono = "font-family=\"ui-monospace,'Cascadia Code',Consolas,monospace\"";
const esc = s => s.replace(/&/g, '&amp;').replace(/</g, '&lt;');

function banner(t) {
  // arka plan ızgarası + "ArnoldGods" + imza amblemi (A harfi şeklinde kalkan)
  let grid = '';
  for (let x = 0; x <= 1200; x += 40) grid += `<line x1="${x}" y1="0" x2="${x}" y2="300" />`;
  for (let y = 0; y <= 300; y += 40) grid += `<line x1="0" y1="${y}" x2="1200" y2="${y}" />`;
  return `<svg xmlns="http://www.w3.org/2000/svg" width="1200" height="300" viewBox="0 0 1200 300" role="img" aria-label="ArnoldGods — AI systems, built in the open.">
  <rect width="1200" height="300" fill="${t.bg}"/>
  <g stroke="${t.line}" stroke-width="1" opacity="0.55">${grid}</g>
  <rect x="0.5" y="0.5" width="1199" height="299" fill="none" stroke="${t.line}"/>
  <g transform="translate(90,70)">
    <path d="M80 0 L160 30 V105 C160 150 125 180 80 200 C35 180 0 150 0 105 V30 Z" fill="${t.card}" stroke="${t.accent}" stroke-width="5"/>
    <path d="M48 140 L80 52 L112 140 M60 112 H100" fill="none" stroke="${t.accent}" stroke-width="10" stroke-linecap="round" stroke-linejoin="round"/>
  </g>
  <text x="300" y="130" ${font} font-size="76" font-weight="700" fill="${t.fg}" letter-spacing="-2">ArnoldGods</text>
  <text x="302" y="178" ${font} font-size="28" fill="${t.mute}">AI systems, built in the open.</text>
  <text x="302" y="225" ${mono} font-size="18" fill="${t.accent}">$ build --test --inspect --share</text>
  <circle cx="1100" cy="60" r="6" fill="${t.accent}"/><circle cx="1124" cy="60" r="6" fill="${t.line}"/><circle cx="1148" cy="60" r="6" fill="${t.line}"/>
</svg>`;
}

const cards = [
  { id: 'agent-hafiza', no: '01 / MEMORY', title: 'agent-hafiza', desc: ['Agent oturumları arasında bağlamı koruyan,', 'Markdown tabanlı yerel ikinci beyin.'] },
  { id: 'claude-skills', no: '02 / WORKFLOWS', title: 'claude-skills', desc: ['Günlük işlerden çıkan, tekrar kullanılabilir', 'ajan becerileri: kod, review, araştırma.'] },
  { id: 'model-arena', no: '03 / EXPERIMENTS', title: 'model-arena', desc: ['Aynı prompt, farklı modeller. Orijinal', 'çıktılar ve maliyet açıkta, kendin incele.'] },
  { id: 'terminal-kit', no: '04 / TOOLS', title: 'terminal-kit', desc: ['Terminal ve çoklu-ajan iş akışları için', 'küçük, hızlı komut satırı araçları.'] },
];

function card(c, t) {
  return `<svg xmlns="http://www.w3.org/2000/svg" width="600" height="190" viewBox="0 0 600 190" role="img" aria-label="${esc(c.title)} — ${esc(c.desc.join(' '))}">
  <rect x="0.5" y="0.5" width="599" height="189" rx="12" fill="${t.card}" stroke="${t.line}"/>
  <rect x="0" y="24" width="4" height="40" fill="${t.accent}"/>
  <text x="28" y="44" ${mono} font-size="13" fill="${t.mute}" letter-spacing="1.5">${c.no}</text>
  <text x="28" y="86" ${font} font-size="30" font-weight="700" fill="${t.fg}">${esc(c.title)}</text>
  <text x="28" y="120" ${font} font-size="16" fill="${t.mute}">${esc(c.desc[0])}</text>
  <text x="28" y="142" ${font} font-size="16" fill="${t.mute}">${esc(c.desc[1])}</text>
  <text x="28" y="172" ${font} font-size="15" font-weight="600" fill="${t.accent}">Depoyu aç →</text>
</svg>`;
}

for (const [name, t] of Object.entries(themes)) {
  fs.writeFileSync(`assets/banner-${name}.svg`, banner(t));
  for (const c of cards) fs.writeFileSync(`assets/card-${c.id}-${name}.svg`, card(c, t));
}
console.log('ok');
