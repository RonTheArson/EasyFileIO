const { FileHandler } = require('./EasyFileIO.dll');

module.exports = {
  readFile: FileHandler.ReadFile,
  readFileAsync: FileHandler.ReadFileAsync,
  writeFile: FileHandler.WriteFile,
  writeFileAsync: FileHandler.WriteFileAsync,
  appendToFile: FileHandler.AppendToFile,
  appendToFileAsync: FileHandler.AppendToFileAsync
};