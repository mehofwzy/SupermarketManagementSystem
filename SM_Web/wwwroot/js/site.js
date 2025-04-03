// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

//V1
//function printBill() {
//    var printContent = document.getElementById("printableBill").innerHTML;
//    var printWindow = window.open('', '', 'width=900,height=700');
//    printWindow.document.write('<html><head><title> Normal Tax Invoice</title>');
//    printWindow.document.write('<style>' +
//        'body { font-family: Arial, sans-serif; margin: 20px; }' +
//        '.bill-details { display: flex; justify-content: space-between; margin-bottom: 10px; }' + // Makes bill details appear in a row
//        'table { width: 100%; border-collapse: collapse; margin-top: 10px; }' +
//        'th, td { border: 1px solid #000; padding: 8px; text-align: left; }' +
//        'th { background-color: #ddd; }' +
//        '@media print {' +
//        'body { margin: 0; padding: 20px; }' +
//        '.no-print { display: none !important; }' + // Hide site links, buttons, etc.
//        'hr { border: 1px solid #000; }' + // Clear separator
//        '}' +
//        '</style></head><body>');
//    printWindow.document.write(printContent);
//    printWindow.document.write('</body></html>');
//    printWindow.document.close();
//    printWindow.print();
//}

//V2
//function printBill() {
//    var originalContent = document.body.innerHTML; // Store full page content
//    var printContent = document.getElementById("printableBill").innerHTML;

//    // Set only the invoice content for printing
//    document.body.innerHTML = `
//    <html>
//    <head>
//        <title>Invoice</title>
//        <style>
//            body { font-family: Arial, sans-serif; margin: 0; padding: 20px; text-align: left; }
//            .bill-details { display: flex; justify-content: space-between; flex-wrap: wrap; margin-bottom: 15px; }
//            .bill-details div { margin: 5px 15px; font-size: 16px; }
//            table { width: 100%; border-collapse: collapse; margin-top: 10px; }
//            th, td { border: 1px solid #000; padding: 8px; text-align: left; }
//            th { background-color: #ddd; }
//            hr { border: 1px solid #000; }
//            @media print {
//                body { margin: 0; padding: 10px; }
//                .no-print, .navbar, .footer { display: none !important; } /* Hide extra elements */
//            }
//        </style>
//    </head>
//    <body>
//        <hr>
//        <div class="bill-details">${printContent}</div>
//    </body>
//    </html>
//    `;

//    window.print(); // Print the page

//    document.body.innerHTML = originalContent; // Restore original content
//    location.reload(); // Reload to restore scripts and event listeners
//}


//V3
//function printBill() {
//    var printContent = document.getElementById("printableBill").innerHTML;
//    var printWindow = window.open('', '', 'width=900,height=700');

//    printWindow.document.write('<html><head><title>Normal Tax Invoice</title>');
//    printWindow.document.write('<style>' +
//        'body { font-family: Arial, sans-serif; margin: 20px; text-align: left; }' +
//        'h2 { margin-bottom: 5px; }' +
//        '.bill-details { display: flex; justify-content: left; flex-wrap: wrap; margin-bottom: 15px; }' +
//        '.bill-details div { margin: 5px 15px 0px 1px; font-size: 16px; }' +
//        'table { width: 100%; border-collapse: collapse; margin-top: 10px; }' +
//        'th, td { border: 1px solid #000; padding: 8px; text-align: center; }' +
//        'th { background-color: #ddd; }' +
//        '@media print {' +
//        'body { margin: 0; padding: 20px; }' +
//        '.no-print { display: none !important; }' + // Hide site links, buttons, etc.
//        'hr { border: 1px solid #000; }' +
//        '}' +
//        '</style></head><body>');

//    printWindow.document.write('<h2>Supermarket Management System</h2>'); // Centered Title
//    printWindow.document.write('<hr>');

//    printWindow.document.write('<div class="bill-details">' + printContent + '</div>');

//    printWindow.document.write('</body></html>');
//    printWindow.document.close();
//    printWindow.print();
//}

//use this
//V4
function printBill() {
    var printContent = document.getElementById("printableBill").innerHTML;
    var printContent1 = document.getElementById("printableBill1").innerHTML;
    var printContent2 = document.getElementById("printableBill2").innerHTML;

    var printWindow = window.open('', '', 'width=900,height=700');

    printWindow.document.write('<html><head><title>Normal Tax Invoice</title>');
    printWindow.document.write('<style>' +
        'body { font-family: Arial, sans-serif; margin: 20px; text-align: left; }' +
        'h2 { text-align: center; margin-bottom: 5px; }' + // Center ONLY the title
        '.bill-details { display: flex; justify-content: left; flex-wrap: wrap; margin-bottom: 15px; }' +
        '.bill-details div { margin: 5px 15px 0px 1px; font-size: 16px; }' +
        'table { width: 100%; border-collapse: collapse; margin-top: 10px; }' +
        'th, td { border: 1px solid #000; padding: 8px; text-align: center; }' +
        'th { background-color: #ddd; }' +
        '@media print {' +
        'body { margin: 0; padding: 20px; }' +
        '.no-print { display: none !important; }' + // Hide site links, buttons, etc.
        'hr { border: 1px solid #000; }' +
        '}' +
        '</style></head><body>');

    printWindow.document.write('<h2>Supermarket Management System</h2>'); // Title is centered
    printWindow.document.write('<hr>');
    //printWindow.document.write('<div class="bill-details">' + printContent + '</div>');
    //printWindow.document.write('<div class="bill-details">' + printContent1 + '<br/><br/>'+ printContent2 + '</div>');
    printWindow.document.write('<div class="bill-details">' + printContent1 + '</div>');
    printWindow.document.write('<div class="bill-details">' + printContent2 + '</div>');

    printWindow.document.write('</body></html>');
    printWindow.document.close();
    printWindow.print();
}
