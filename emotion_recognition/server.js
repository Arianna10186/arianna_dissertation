const dgram = require('dgram');
const express = require('express');
const cors = require('cors');
const bodyParser = require('body-parser');

const app = express();
const udpClient = dgram.createSocket('udp4');

app.use(cors());
app.use(bodyParser.json());

const UDP_SERVER_IP = '127.0.0.1';
// const UDP_SERVER_IP = '10.192.78.205';
const UDP_SERVER_PORT = 6000;

app.post('/detect', (req, res) => {
    const faceData = JSON.stringify(req.body);
    console.log(faceData);

    udpClient.send(faceData, UDP_SERVER_PORT, UDP_SERVER_IP, (err) => {
        if (err) console.error('Error sending data', err);
    });
    res.status(200).send('success');
});
const port = 5000;
app.listen(port, () => {
    console.log(`Listening at http://localhost:${port}`)
});