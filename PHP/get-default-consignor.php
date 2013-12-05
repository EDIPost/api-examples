<?php

// Import some common code
require_once('common.php');


/*
 * Print consignment
 */
function getDefaultConsignor() {
	$url = BASE_URL . '/consignor/default';

	$headers = array(
		'Accept: application/vnd.edipost.party+xml'
	);

	$result = getRequest( $url, API_KEY, $headers );
	$xml = simplexml_load_string( $result );

	return $xml;
}


/*
 * Run the PHP script
 */
$consignor = getDefaultConsignor();

echo 'Consignor ID: ' . $consignor->attributes()->id . '<br>';
echo 'Consignor Name: ' . $consignor->companyName;

?>