import { CountryCode } from "./country-code";

export type CountryFactory<PropertyValue = unknown> = Record<CountryCode, PropertyValue>;
