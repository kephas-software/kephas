
// Hack to work around https://github.com/aspnet/AspNetCore/issues/17277

const { spawn } = require('child_process');

const portArgIndex = process.argv.indexOf('--port');

if (portArgIndex === -1) {
    throw new Error('Could not detect port number');
}

const port = process.argv[portArgIndex + 1];

// console.log(`open your browser on http://localhost:${port}`);

const child = spawn(`cmd`, ['/c', `npm.cmd run start:ng -- --port ${port}`]);

console.log('Angular CLI started on PID', child.pid);

child.stdout.on('data', x => console.log(x && x.toString()));

child.stderr.on('data', x => console.error(x && x.toString()));

const sleep = () => {
    console.log('[Node.js keepalive]');
    setTimeout(sleep, 10000);
}

sleep();
