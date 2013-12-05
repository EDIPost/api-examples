<?php

// Import some common code
require_once('common.php');


/*
 * Create consingnment
 */
function createConsignment() {
	$body = file_get_contents('consignment.xml');

	$url = BASE_URL . '/consignment';

	$headers = array(
		'Accept: application/vnd.edipost.consignment+xml',
		'Content-Type: application/vnd.edipost.consignment+xml'
	);

	$result = postRequest( $url, API_KEY, $headers, $body );
	$xml = simplexml_load_string( $result );

	return $xml;
}


/*
 * Run the PHP script
 */
$consignment = createConsignment();


echo '<h1>Shipment</h1>';
echo 'ID: ' . $consignment->attributes()->id . '<br>';
echo 'Shipment number: ' . $consignment->shipmentNumber . '<br><br>';

echo '<h1>Shipment items</h1>';

foreach( $consignment->items->item as $item ) {
	echo 'Item number: ' . $item->itemNumber . '<br>';
	echo 'Weight: ' . $item->weight . '<br>';
	echo 'Weight: ' . $item->cost . '<br><br>';
}

?>