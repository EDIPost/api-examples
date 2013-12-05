<?php

// Import some common code
require_once('common.php');


/*
 * Print consignment
 */
function printConsignment( $consignmentId ) {
	$url = BASE_URL . '/consignment/' . $consignmentId . '/print';

	$headers = array(
		'Accept: application/pdf'
	);

	return getRequest( $url, API_KEY, $headers );
}


/*
 * Run the PHP script
 */
$shipmentId = 875157;	// The ID returned when creating the consignment
$pdf = printConsignment( $shipmentId );

header('Content-Type: application/pdf');
header('Content-Disposition: attachment; filename="label.pdf"');
echo $pdf;

?>