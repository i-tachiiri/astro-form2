'use client';
import { Navbar, NavbarBrand } from '@nextui-org/navbar';

export default function Header() {
  return (
    <Navbar className="mb-4">
      <NavbarBrand>
        <h1 className="font-bold text-lg">Astro Search</h1>
      </NavbarBrand>
    </Navbar>
  );
}
