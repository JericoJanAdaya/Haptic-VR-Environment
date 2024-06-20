import asyncio
import subprocess
import sys
# Ensure bleak is installed
try:
    import bleak
except ImportError:
    subprocess.check_call([sys.executable, "-m", "pip", "install", "bleak"])
    import bleak
from bleak import BleakScanner, BleakClient

address = "12:E9:00:28:AD:83"  # Replace with your BLE device address

async def scan_and_connect():
    print("Scanning for BLE devices...")
    devices = await BleakScanner.discover()
    for device in devices:
        print(f"Device found: {device}")

    print(f"Connecting to {address}...")
    async with BleakClient(address) as client:
        services = await client.get_services()
        for service in services:
            print(f"Service: {service}")
            for characteristic in service.characteristics:
                print(f"  Characteristic: {characteristic} - {characteristic.properties}")

if __name__ == "__main__":
    asyncio.run(scan_and_connect())
