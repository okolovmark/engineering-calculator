var express = require('express');
var app = express();
const fs = require('mz/fs');

app.post('/', function (request, respond) {
    var body = '';
    var filePath = "files/history.txt";
    request.on('data', function (data) {
        body += data;
    });

    request.on('end', function () {
        fs.appendFile(filePath, body, function () {
            respond.end();
        });
    });
});



app.get('/', function (req, res) {

    fs.readFile("files/history.txt", "utf8", function (err, contents) {
        console.log(contents);
        res.send(contents);
    });
});

app.listen(3000, function () {
    console.log('run on port 3000!');
});