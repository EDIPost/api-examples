<?php

// Import some common code
require_once('common.php');


/*
 * Print consignment
 */
function findProducts( $consigneeId ) {
	$url = BASE_URL . '/consignee/' . $consigneeId . '/products';

	$headers = array(
		'Accept: application/vnd.edipost.collection+xml'
	);

	$result = getRequest( $url, API_KEY, $headers );
	$xml = simplexml_load_string( $result );

	return $xml;
}


/*
 * Run the PHP script
 */
$consigneeId = 1504551;	// The ID returned when creating the consignee
$products = findProducts( $consigneeId );


foreach ( $products->entry as $product ) {
	echo 'Product: ' . $product->attributes()->name . ' (' . $product->attributes()->id . ')<br>';
	echo 'Status: ' . $product->status . '<br>';

	foreach( $product->services->service as $service ) {
		echo 'Service: ' . $service->attributes()->name . ' (' . $service->attributes()->id . ')<br>';
	}

	echo '<br>';
}


?>