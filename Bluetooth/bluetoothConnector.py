import asyncio
import serial.tools.list_ports
from bleak import BleakClient

address = "12:E9:00:28:AD:83"

def list_com_ports():
    ports = serial.tools.list_ports.comports()
    for port in ports:
        print(f"Device: {port.device}, Description: {port.description}")

async def discover_services(address):
    async with BleakClient(address) as client:
        services = await client.get_services()
        for service in services:
            print(f"Service: {service}")
            for characteristic in service.characteristics:
                print(
                    f"  Characteristic: {characteristic} - {characteristic.properties}"
                )

# Discover BLE services
asyncio.run(discover_services(address))

# List COM ports
list_com_ports()
