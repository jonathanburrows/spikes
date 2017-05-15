import { MaterialLayoutPage } from './app.po';

describe('material-layout App', () => {
  let page: MaterialLayoutPage;

  beforeEach(() => {
    page = new MaterialLayoutPage();
  });

  it('should display message saying app works', () => {
    page.navigateTo();
    expect(page.getParagraphText()).toEqual('app works!');
  });
});
