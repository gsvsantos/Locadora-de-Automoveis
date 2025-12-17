const fs = require('fs');
const path = require('path');

function requireEnv(variableName) {
  const variableValue = process.env[variableName];
  if (!variableValue || !variableValue.trim()) {
    throw new Error(`Missing env var: ${variableName}`);
  }
  return variableValue.trim();
}

const apiUrl = requireEnv('APIURL');
const googleClientId = requireEnv('CLIENT_ID');

const environmentFileContent =
  `export const environment = {\n` +
  `  production: true,\n` +
  `  apiUrl: ${JSON.stringify(apiUrl)},\n` +
  `  client_id: ${JSON.stringify(googleClientId)}\n` +
  `} as const;\n`;

const environmentFilePath = path.join(__dirname, '..', 'src', 'environments', 'environment.ts');
fs.writeFileSync(environmentFilePath, environmentFileContent, 'utf8');
