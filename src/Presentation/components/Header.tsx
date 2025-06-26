'use client';
import { Navbar, NavbarBrand } from '@heroui/navbar';
import { Globe } from 'lucide-react';

export default function Header() {
  return (
    <Navbar className="mb-4">
      <NavbarBrand className="gap-2">
        <Globe className="w-5 h-5" />
        <h1 className="font-bold text-lg">Astro Search</h1>
      </NavbarBrand>
    </Navbar>
  );
}
