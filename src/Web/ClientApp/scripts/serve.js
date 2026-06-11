const { spawnSync } = require('child_process');

const port = process.env.PORT || '44447';
const result = spawnSync('ng', ['serve', '--port', port], { stdio: 'inherit', shell: true });
process.exit(result.status ?? 1);
