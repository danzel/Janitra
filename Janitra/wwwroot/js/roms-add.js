document.getElementById('scan-rom').addEventListener('click',
    function(e) {
        e.preventDefault();

        document.getElementById('rom-scanner').click();
    },
    false);
document.getElementById("rom-scanner").addEventListener('change', scanRom, false);

//TODO: This should use webworkers so we don't lock the browser up

function scanRom(evt) {
    if (!evt.target.files) {
        return;
    }
    var files = evt.target.files;
    var file = files[0];

    document.getElementById('file-name').value = file.name;
    document.getElementById('scan-rom').disabled = "disabled";

    var shaObj = new jsSHA("SHA-256", "ARRAYBUFFER");

    //Calculate the hash
    var at = 0;
    var chunkSize = 10 * 1000 * 1000; //10MB

    var reader = new FileReader();
    reader.onload = function() {
        shaObj.update(reader.result);


        if (at < file.size) {
            document.getElementById('scan-rom').innerHTML = Math.round(100 * at / file.size) + "%";

            var start = at;
            at = Math.min(file.size, at + chunkSize);

            reader.readAsArrayBuffer(file.slice(start, at));
        } else {
            document.getElementById('file-hash').value = shaObj.getHash('HEX');

            document.getElementById('scan-rom').disabled = "";
            document.getElementById('scan-rom').innerHTML = "Scan Rom";
        }
    }

    at = Math.min(file.size, at + chunkSize);
    reader.readAsArrayBuffer(file.slice(0, at));
}