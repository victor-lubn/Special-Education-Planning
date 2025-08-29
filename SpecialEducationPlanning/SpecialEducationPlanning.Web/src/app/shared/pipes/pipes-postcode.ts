import { Pipe, PipeTransform } from "@angular/core";
import { CountryControllerBase } from "../../core/services/country-controller/country-controller-base";

@Pipe({ name: 'tdpPostcode' })
export class TdpPostCodePipe implements PipeTransform {

  transform(postCode: string, fromController?: boolean, service?: CountryControllerBase): string {
    if (!postCode) {
      return fromController ? '' : '-';
    }

    if (!service && service.getPostCodeTransform) {
      return fromController ? '' : '-';
    }
    return service.getPostCodeTransform().transform(postCode, fromController);
  }
}

@Pipe({ name: 'tdpPostcodeGBR' })
export class TdpPostCodePipeGBR implements PipeTransform {

  transform(postCode: string, fromController?: any) {
    if (!postCode) {
      return fromController ? '' : '-';
    }
    const cleanPostcode = postCode.replace(/\s+/g, '');
    switch (cleanPostcode.length) {
      case 0:
        return fromController ? '' : '-';
      case 5:
        return `${cleanPostcode.slice(0, 2)} ${cleanPostcode.slice(2, cleanPostcode.length)}`;
      case 6:
        return `${cleanPostcode.slice(0, 3)} ${cleanPostcode.slice(3, cleanPostcode.length)}`;
      case 7:
      case 8:
        return `${cleanPostcode.slice(0, 4)} ${cleanPostcode.slice(4, cleanPostcode.length)}`;
      default:
        return postCode;
    }
  }
}

@Pipe({ name: 'tdpPostcodeIRL' })
export class TdpPostCodePipeIRL implements PipeTransform {

  transform(postCode: string, fromController?: any) {
    if (!postCode) {
      return fromController ? '' : '-';
    }
    const cleanPostcode = postCode.replace(/\s+/g, '');
    switch (cleanPostcode.length) {
      case 7:
        return `${cleanPostcode.slice(0, 3)} ${cleanPostcode.slice(3, cleanPostcode.length)}`;
      default:
        return postCode;
    }
  }
}

@Pipe({ name: 'tdpPostcodeFRA' })
export class TdpPostCodePipeFRA implements PipeTransform {

  transform(postCode: string, fromController?: any) {
    if (!postCode) {
      return fromController ? '' : '-';
    }
    const cleanPostcode = postCode.replace(/\s+/g, '');

    if (cleanPostcode) {
      switch (cleanPostcode.length) {
        case 5:
          return cleanPostcode;
        default:
          return postCode;
      }
    }
  }
}