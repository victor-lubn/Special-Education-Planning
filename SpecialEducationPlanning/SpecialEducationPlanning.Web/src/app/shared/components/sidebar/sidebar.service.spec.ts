import { SidebarService } from './sidebar.service';

describe('SidebarService', () => {
  let service: SidebarService;

  beforeEach(() => { 
    service = new SidebarService(); 
  });

  describe('should have a register', () => {
    it('to register if not exists', () => {
      service.register('test', {name: 1});
      expect(service.getSidebar('test')).toBeDefined();
    });

    it('give error if exists', () => {
      spyOn(console, 'error');
      service.register('test', {name: 1});
      service.register('test', {name: 2});
      expect(console.error).toHaveBeenCalled();
    });
  });

  describe('should have a unregister', () => {
    it('to unregister if exists', () => {
      service.register('test', {name: 1});
      service.unregister('test');
      expect(service.getSidebar('test')).not.toBeDefined();
    });

    it('give warning if not exists', () => {
      spyOn(console, 'warn');
      service.unregister('test');
      expect(console.warn).toHaveBeenCalled();
    });
  });
});
