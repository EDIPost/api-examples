#!/usr/bin/env python3

import sys
import requests
import xml.dom.minidom as minidom
import xml.etree.ElementTree as et
import argparse


#
# Pretty print XML for debugging
#
def prettyPrint( xml ):
    print( minidom.parseString( xml ).toprettyxml() )


#
# Get default consignor ID
#
def getDefaultConsignorID():
    url = server + '/consignor/default'
    response = requests.get(url, auth=auth)
    response.encoding = 'UTF-8'

    if response.status_code != 200:
        print('Unable to get default consignor: {}'.format(response.text))
        sys.exit(1)

    return et \
        .fromstring( response.text ) \
        .find('.') \
        .get('id')


#
# Create consignee
#
def createConsingee( data ):
    template = '''<?xml version="1.0" encoding="UTF-8"?>
        <consignee>
          <companyName>{companyName}</companyName>
          <customerNumber>hei</customerNumber>
        
          <postAddress>
            <address>{address1}</address>
            <address2>{address2}</address2>
            <zipCode>{zipCode}</zipCode>
            <city>{city}</city>
          </postAddress>
        
          <streetAddress>
            <address>{address1}</address>
            <address2>{address2}</address2>
            <zipCode>{zipCode}</zipCode>
            <city>{city}</city>
          </streetAddress>
        
          <country>NO</country>
        
          <contact>
            <name></name>
            <email>{email}</email>
            <telephone></telephone>
            <cellphone>{phone}</cellphone>
            <telefax></telefax>
          </contact>
        </consignee>'''

    url = server + '/consignee'
    headers = {'Content-Type': 'application/vnd.edipost.party+xml'}
    response = requests.post(url, data=template.format(**data).encode('utf-8'), auth=auth, headers=headers)
    response.encoding = 'UTF-8'

    if response.status_code != 201:
        print('Unable to create consignee')
        sys.exit(1)

    return et \
        .fromstring( response.text ) \
        .find('.') \
        .get('id')


#
# Create consignment
#
def createConsignment( data ):
    template = '''<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
        <consignment>
          <consignor id="{consignorID}" />
          <consignee id="{consigneeID}" />
        
          <items>
            <item>
              <weight>1.0</weight>
              <height>0.0</height>
              <length>0.0</length>
              <width>0.0</width>
            </item>
          </items>
        
          <product id="{productID}">
            <services>
              <service id="5">
                <properties>
                  <property key="EMSG_SMS_NUMBER" value="{phone}"/>
                </properties>
              </service>
              
              <service id="6">
                <properties>
                  <property key="EMSG_EMAIL" value="{email}"/>
                </properties>
              </service>
            </services>
          </product>
        
          <contentReference></contentReference>
        </consignment>'''

    url = server + '/consignment'
    headers = {'Content-Type': 'application/vnd.edipost.consignment+xml'}
    response = requests.post(url, data=template.format(**data).encode('utf-8'), auth=auth, headers=headers)
    response.encoding = 'UTF-8'

    if response.status_code != 201:
        print('Unable to create consignment')
        sys.exit(1)

    xml = et.fromstring( response.text )
    consignmentID = xml.find('.').get('id')
    shipmentNumber = xml.find('./shipmentNumber').text

    return (consignmentID, shipmentNumber)


#
# Print consignment
#
def printConsignment( id, file ):
    url = server + '/consignment/{id}/print'.format(id=id)
    headers = {'Accept': 'application/pdf'}
    response = requests.get(url, auth=auth, headers=headers, stream=True)

    if response.status_code == 200:
        with open(file, 'wb') as f:
            for chunk in response.iter_content(1024):
                f.write(chunk)
    else:
        print('Unable to print consignment')
        sys.exit(1)






#
# Parse command line parameters
#
parser = argparse.ArgumentParser()
parser.add_argument('apikey', help='Account API key')
parser.add_argument('name', help='Recipient name')
parser.add_argument('address1', help='Recipient address line 1')
parser.add_argument('address2', help='Recipient address line 1')
parser.add_argument('zipcode', help='Recipient zip code')
parser.add_argument('city', help='Recipient city')
parser.add_argument('email', help='Recipient email address')
parser.add_argument('phone', help='Recipient phone')
parser.add_argument('product', help='ID of shipping product')
parser.add_argument('-f', '--file', nargs='?', default='out.pdf', help='Label file name')

if len(sys.argv) == 1:
    parser.print_help(sys.stderr)
    sys.exit(1)

args = parser.parse_args()



#
# Create the shipment
#
server = 'http://apitest.edipost.no'
auth=('api', args.apikey)

consignorID = getDefaultConsignorID()

data = {
    'companyName': args.name,
    'address1'   : args.address1,
    'address2'   : args.address2,
    'zipCode'    : args.zipcode,
    'city'       : args.city,
    'email'      : args.email,
    'phone'      : args.phone
}

consigneeID = createConsingee( data )

data = {
    'consignorID': consignorID,
    'consigneeID': consigneeID,
    'productID'  : args.product,
    'email'      : args.email,
    'phone'      : args.phone
}

(consignmentID, shipmentNumber) = createConsignment( data )

printConsignment( consignmentID, args.file )

print( shipmentNumber )