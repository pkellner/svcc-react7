module.exports = function (callback) {
    // In this trivial example, we don't need to receive any 
    // parameters - we just send back a string 

    var message = 'Hello from Node js script at ' + new Date().toString() + ' process.versions ' + process.version;
    callback(/* error */ null, message);

};
