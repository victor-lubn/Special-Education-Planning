import { TdpDatePipe, SearchFilterPipe, TdpDateSuffixPipe } from './pipes';


import { NgModule } from '@angular/core';
import { DatePipe } from '@angular/common';
import { TdpPostCodePipe, TdpPostCodePipeFRA, TdpPostCodePipeGBR, TdpPostCodePipeIRL } from './pipes-postcode';

@NgModule({
    declarations: [
        TdpDatePipe,
        TdpDateSuffixPipe,
        TdpPostCodePipe,
        TdpPostCodePipeGBR,
        TdpPostCodePipeIRL,
        TdpPostCodePipeFRA,
        SearchFilterPipe
    ],
    exports: [
        TdpDatePipe,
        TdpDateSuffixPipe,
        TdpPostCodePipe,
        TdpPostCodePipeGBR,
        TdpPostCodePipeIRL,
        TdpPostCodePipeFRA,
        SearchFilterPipe
    ],
    providers: [
        DatePipe,
        TdpPostCodePipe,
        TdpPostCodePipeGBR,
        TdpPostCodePipeIRL,
        TdpPostCodePipeFRA
    ]
}
)
export class PipeModule {}

