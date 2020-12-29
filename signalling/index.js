
'use strict';

var os = require('os');
const fs = require('fs');
var nodeStatic = require('node-static');
var https = require('https');
var socketIO = require('socket.io');
//const ortc = require('./ortc.js');
const config = require('./config');
const express = require('express');

//const uuidV4 = require('uuid/v4');
////import { v4 as uuidV4 } from 'uuid';
const { v4: uuidV4 } = require('uuid');


let webServer;
let socketServer;
let expressApp;
let io;

(async () => {
  try {
    await runExpressApp();
    await runWebServer();
    await runSocketServer();
  } catch (err) {
    console.error(err);
  }
})();



console.log("https://localhost:8080/");

let serverSocketid =null;


var fileServer = new(nodeStatic.Server)();
// var app = http.createServer(function(req, res) {
//  fileServer.serve(req, res);
// }).listen(8080);

async function runExpressApp() {
  expressApp = express();
  expressApp.use(express.json());
  expressApp.use(express.static(__dirname));

  expressApp.use((error, req, res, next) => {
    if (error) {
      console.log('Express app error,', error.message);

      error.status = error.status || (error.name === 'TypeError' ? 400 : 500);

      res.statusMessage = error.message;
      res.status(error.status).send(String(error));
    } else {
      next();
    }
  });
}

async function runWebServer() {

  console.error('runWebServer');

  const { sslKey, sslCrt } = config;
  if (!fs.existsSync(sslKey) || !fs.existsSync(sslCrt)) {
    console.error('SSL files are not found. check your config.js file');
    process.exit(0);
  }
  const tls = {
    cert: fs.readFileSync(sslCrt),
    key: fs.readFileSync(sslKey),
  };
  webServer = https.createServer(tls, expressApp);
  webServer.on('error', (err) => {
    console.error('starting web server failed:', err.message);
  });

  
  await new Promise((resolve) => {
    const { listenIp, listenPort } = config;
    webServer.listen(listenPort, listenIp, () => {
      console.log('server is running');
      console.log(`open https://127.0.0.1:${listenPort} in your web browser`);

    //  listenIps = config.webRtcTransport.listenIps;
      //const ip = listenIps.announcedIp || listenIps.ip;
     // console.log('listen ips ' + JSON.stringify(listenIps, null, 4) );

      resolve();
    });
  });
}


function log() {
    var array = ['Message from server:'];
    array.push.apply(array, arguments);
   // socket.emit('log', array);
    console.log(array);
  }


async function runSocketServer() {

    console.error('runSocketServer');


   io = socketIO.listen(webServer);

   io.sockets.on('connection', function(socket) {


  // convenience function to log server messages on the client


  socket.on('message', function(message) {
    
    // for a real app, would be room-only (not broadcast)
    //socket.broadcast.emit('message', message);
     message.from = socket.id;

    if ('to' in message) {
      log('Client said: ', message);
      socket.to(message.to).emit('message', message);
    }
    else
    {


      var clientsInRoom = io.sockets.adapter.rooms[message.room];
      var numClients = clientsInRoom ? Object.keys(clientsInRoom.sockets).length : 0;

      console.log("Number of participant " + numClients );

      if(numClients ===1)
      {
         log('Client said: ', message);
        return;
      }

      
      

      console.log("message.from "+ message.from);
      for( const member in clientsInRoom.sockets ) 
      {

        if( member !==  message.from ) {
          message.to = member;
          log('Client said: ', message);
          socket.to(message.to).emit('message', message);
         
        }
      }

    }
    

  });

  socket.on('create or join', function(roomId) {
    log('Received request to create or join room ' + roomId);

    socket.join(roomId);

    var numClients = io.sockets.adapter.rooms[roomId].length;  //For socket.io versions >= 1.4:

    log('Room ' + roomId + ' now has ' + numClients + ' client(s)');

    if (numClients === 1 ) {
     
      log('Client ID ' + socket.id + ' created room ' + roomId);
    


      socket.emit('created', roomId, socket.id);

      socket.emit('join', roomId, socket.id);

    } else if (numClients > 1 ) {
      log('Client ID ' + socket.id + ' joined room ' + roomId);
      io.sockets.in(roomId).emit('join', roomId, socket.id);

      socket.emit('joined', roomId, socket.id);
      io.sockets.in(roomId).emit('ready');
    } 




  });

  socket.on('ipaddr', function() {
    var ifaces = os.networkInterfaces();
    for (var dev in ifaces) {
      ifaces[dev].forEach(function(details) {
        if (details.family === 'IPv4' && details.address !== '127.0.0.1') {
          socket.emit('ipaddr', details.address);
        }
      });
    }
  });

  socket.on('bye', function() {
    console.log('received bye');
  });

  //////

  socket.on('disconnect', function() {
    console.log("disconnect " + socket.id);

    //   for( let soc in  io.sockets.connected ){
    //     io.sockets.connected[soc].disconnect();
    // }

    });
  ////

 });


}
