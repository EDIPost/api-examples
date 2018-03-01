# Python examples

Examples on how to use the Edipost API using the Python programming language.


## Usage

Display help page

```
./create-shipment.py -h
usage: create-shipment.py [-h] [-f [FILE]]
                          apikey name address1 address2 zipcode city product

positional arguments:
  apikey                Account API key
  name                  Recipient name
  address1              Recipient address line 1
  address2              Recipient address line 1
  zipcode               Recipient zip code
  city                  Recipient city
  product               ID of shipping product

optional arguments:
  -h, --help            show this help message and exit
  -f [FILE], --file [FILE]
                        Label file name
```

Create a consignment for "Ola Normann" and save the shipping label as "etikett.pdf"

```
./create-shipment.py "0e9afc2b3a861d786272026e448h1325fd6069b4" "Ola Normann" "Portveien 2" "Oppgang 3" "1337" "Sandvika" 8 -f etikett.pdf
```
