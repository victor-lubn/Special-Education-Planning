import { GBRservice } from "../countries/gbr.service";
import { IRLservice } from "../countries/irl.service";
import { FRAservice } from "../countries/fra.service";

export const countries = [
    {id: 'IRL', service: IRLservice},
    {id: 'GBR', service: GBRservice},
    {id: 'FRA', service: FRAservice}];