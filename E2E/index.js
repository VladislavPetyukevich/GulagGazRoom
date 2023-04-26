const http = require("http");
const fs = require('fs');
const path = require('path');
const { spawn } = require('child_process');

const host = 'localhost';
const port = 8888;

const backendUrl = 'http://localhost:5043/swagger/index.html';

const projectParh = path.resolve('../Backend/Interview.Backend');
const dbPath = path.resolve('../Backend/Interview.Backend/database-conf');

const silentDeleteDB = () => {
  try {
    fs.unlinkSync(dbPath);
  } catch { }
};

const spawnDotnetProcess = () => {
  const process = spawn('dotnet', ['run', '--project', `${projectParh}`]);

  process.stdout.on('data', (data) => {
    console.log(`stdout:\n${data}`);
  });

  process.stderr.on('data', (data) => {
    console.error(`stderr: ${data}`);
  });

  process.on('error', (error) => {
    console.error(`error: ${error.message}`);
  });

  return process;
}

const waitProcessToClose = (process) =>
  new Promise(resolve => {
    process.on('close', resolve);
    process.kill();
  });

const waitForAvailability = (url) =>
  new Promise(resolve => {
    const intervalMs = 1000;
    const requestTimeout = 1000;
    const maxAttemptsCount = 10;
    let attemptsCount = 0;
    const makeAttempt = () => {
      attemptsCount++;
      if (attemptsCount > maxAttemptsCount) {
        throw new Error('cant wait for backend availability')
      }
      const req = http.get(
        url,
        { timeout: requestTimeout },
        resolve
      );
      req.on('timeout', () => {
        setTimeout(makeAttempt, intervalMs);
      });
      req.on('error', () => {
        setTimeout(makeAttempt, intervalMs);
      });
    };
    makeAttempt();
  });

let dotnetProcess = null;

const nukeBackend = async () => {
  if (dotnetProcess) {
    await waitProcessToClose(dotnetProcess);
  }
  silentDeleteDB();
  dotnetProcess = spawnDotnetProcess();
};

const requestListener = async (req, res) => {
  try {
    if (
      (req.url !== '/nukeBackend') ||
      (req.method !== 'POST')
    ) {
      res.writeHead(404);
      res.end();
      return;
    }
    await nukeBackend();
    await waitForAvailability(backendUrl);
    res.writeHead(200);
    res.end();
  } catch (err) {
    res.writeHead(500);
    res.end(err.message);
  }
};

const server = http.createServer(requestListener);
server.listen(port, host, () => {
  console.log(`Server is running on http://${host}:${port}`);
});
