<?php

// Import some common code
require_once('common.php');


/*
 * Create consingee
 */
function createConsignee() {
	$body = file_get_contents('consignee.xml');

	$url = BASE_URL . '/consignee';

	$headers = array(
		'Accept: application/vnd.edipost.party+xml',
		'Content-Type: application/vnd.edipost.party+xml'
	);

	$result = postRequest( $url, API_KEY, $headers, $body );
	$xml = simplexml_load_string( $result );

	return $xml;
}


/*
 * Run the PHP script
 */
$consignee = createConsignee();
echo 'ID on the new consingee: ' . $consignee->attributes()->id;

?>