const {app, BrowserWindow} = require("electron");

let mainWindow;

const createWindow = () => {

  mainWindow = new BrowserWindow({
    simpleFullscreen :true,
    center:true,
    autoHideMenuBar:true,
    nodeIntegrationInSubFrames:true
  })

  mainWindow.loadFile("dist/index.html").then();

  mainWindow.on("closed", () => {
    mainWindow = null;
  });

  mainWindow.webContents.on('did-fail-load', (event, error) => {
    if(error === -6) {
      mainWindow.loadFile("dist/index.html").then();
    }
  });

  mainWindow.webContents.on("did-create-window", (newWindow) =>{
    newWindow.center();
    newWindow.setMenuBarVisibility(false);
  });

}

app.on("ready", () => {
  createWindow();
});
app.on("window-all-closed",() => {
  if(process.platform !== "darwin"){
    app.quit();
  }
});
app.on("activate", () => {
  if(window === null){
    createWindow();
  }
})




