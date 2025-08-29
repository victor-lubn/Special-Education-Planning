import { ComponentFixture, TestBed } from '@angular/core/testing';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { IconComponent } from '../atoms/icon/icon.component';

import { SidebarComponent } from './sidebar.component';
import { SidebarService } from './sidebar.service';

describe('SidebarComponent', () => {
  let component: SidebarComponent;
  let fixture: ComponentFixture<SidebarComponent>;
  let sidebarService: SidebarService;

  const sidebarServiceMock = {
    register: () => null,
    unregister: () => null
  }

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ BrowserAnimationsModule ],
      declarations: [ SidebarComponent, IconComponent ],
      providers: [
        {provide: SidebarService, useValue: sidebarServiceMock}
     ],
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(SidebarComponent);
    component = fixture.componentInstance;
    sidebarService = TestBed.get(SidebarService);
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should have the default values', () => {
    expect(component.position).toBe('right');
    expect(component.animationDelay).toBe(150);
  });

  it('should have ngOnInit', () => {
    spyOn(sidebarService, 'register');
    component.folded = true;
    component.opened = true;
    fixture.detectChanges();
    component.ngOnInit();

    expect(sidebarService.register).toHaveBeenCalled();
  });

  it('should have ngOnDestroy', () => {
    spyOn(sidebarService, 'unregister');
    component.folded = true;
    component.opened = true;
    fixture.detectChanges();
    component.ngOnDestroy();

    expect(sidebarService.unregister).toHaveBeenCalled();
  });

  describe('should have set to folded', () => {
    beforeEach(() => {
      spyOn(component.foldedChanged, 'emit');
    });

    it('and do nothing if already opened', () => {
      component.opened = false;
      fixture.detectChanges();
      
      component.folded = true;
      expect(component.foldedChanged.emit).not.toHaveBeenCalled();
    });

    describe('and set sibling styles', () => { 
      beforeEach(() => {
        component.opened = true;
      });

      it('for position left', () => {
        component.position = 'left';
        fixture.detectChanges();

        component.folded = true;
        expect(component.foldedChanged.emit).not.toHaveBeenCalled();
      });

      describe('for position right', () => {
        beforeEach(() => {
          component.position = 'right';
          component.opened = true;
        });

        it('for folded true', () => {
          fixture.detectChanges();
  
          component.folded = true;
          expect(component.foldedChanged.emit).toHaveBeenCalled();
        });
  
      });
    });
  });

  describe('should have a method to open sidebar', () => {
    beforeEach(() => {
      spyOn(component.openedChanged, 'emit');
      component.opened = false;
    });

    it('and do nothing if already opened', () => {
      component.opened = true;
      fixture.detectChanges();
      
      component.open();
      expect(component.openedChanged.emit).not.toHaveBeenCalled();
    });

    it('and open sidebar', () => { 
      component.open();
      expect(component.openedChanged.emit).toHaveBeenCalled();
    });
  });

  describe('should have a method to close sidebar', () => {
    beforeEach(() => {
      spyOn(component.openedChanged, 'emit');
      component.opened = true;
    });

    it('and do nothing if already closed', () => {
      component.opened = false;
      fixture.detectChanges();
      
      component.close();
      expect(component.openedChanged.emit).not.toHaveBeenCalled();
    });

    it('and close sidebar', () => { 
      component.close();
      expect(component.openedChanged.emit).toHaveBeenCalled();
    });
    
  });

  describe('should have a method to toggle sidebar', () => {
    beforeEach(() => {
      spyOn(component.openedChanged, 'emit');
    });

    it('and should close sidebar', () => {
      component.opened = true;
      fixture.detectChanges();
      
      component.toggleOpen();
      expect(component.openedChanged.emit).toHaveBeenCalled();
    });

    it('and should open sidebar', () => {
      component.opened = false;
      fixture.detectChanges();
      
      component.toggleOpen();
      expect(component.openedChanged.emit).toHaveBeenCalled();
    });
  });

  describe('should have a method to fold sidebar', () => {
    beforeEach(() => {
      component.folded = false;
    });

    it('and do nothing if already folded', () => {
      component.folded = true;
      fixture.detectChanges();
      
      component.fold();
      expect(component.folded).toBeTrue();
    });

    it('and fold sidebar', () => { 
      component.fold();
      expect(component.folded).toBeTrue();
    });
  });

  describe('should have a method to unfold sidebar', () => {
    beforeEach(() => {
      component.folded = true;
    });

    it('and do nothing if already unfolded', () => {
      component.folded = false;
      fixture.detectChanges();
      
      component.unfold();
      expect(component.folded).toBeFalse();
    });

    it('and unfold sidebar', () => { 
      component.unfold();
      expect(component.folded).toBeFalse();
    });
  });

  describe('should have a method to toggleFold sidebar', () => {
    it('and should fold sidebar', () => {
      component.folded = false;
      fixture.detectChanges();
      
      component.toggleFold();
      expect(component.folded).toBeTrue();
    });

    it('and should unfold sidebar', () => {
      component.folded = true;
      fixture.detectChanges();
      
      component.toggleFold();
      expect(component.folded).toBeFalse();
    });
  });

  describe('should have a method to foldTemporarily sidebar', () => {
    beforeEach(() => {
      component.folded = true;
    });

    it('and do nothing if already unfolded', () => {
      component.folded = false;
      fixture.detectChanges();
      
      component.foldTemporarily();
      expect(component.folded).toBeFalse();
    });

    it('and fold sidebar', () => { 
      component.foldTemporarily();
      expect(component.unfolded).toBeFalse();
    });
  });

  describe('should have a method to unfoldTemporarily sidebar', () => {
    beforeEach(() => {
      component.folded = true;
    });

    it('and do nothing if already unfolded', () => {
      component.folded = false;
      fixture.detectChanges();
      
      component.unfoldTemporarily();
      expect(component.folded).toBeFalse();
    });

    it('and unfold sidebar', () => { 
      component.unfoldTemporarily();
      expect(component.unfolded).toBeTrue();
    });
  });
});
