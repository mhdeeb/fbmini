import { Pipe, PipeTransform } from '@angular/core';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import LinkifyIt from 'linkify-it';

@Pipe({
  name: 'linkify',
})
export class LinkifyPipe implements PipeTransform {
  private readonly linkify = new LinkifyIt();

  constructor(private readonly sanitizer: DomSanitizer) {}

  transform(text: string): SafeHtml {
    if (!text) {
      return '';
    }

    const matches = this.linkify.match(text);

    if (!matches) {
      return text;
    }

    let result = '';
    let lastIndex = 0;

    for (const match of matches) {
      result += text.slice(lastIndex, match.index);

      const url = match.url;
      result += `<a href="${url}" target="_blank" rel="noopener noreferrer">${match.raw}</a>`;

      lastIndex = match.lastIndex;
    }

    result += text.slice(lastIndex);

    return this.sanitizer.bypassSecurityTrustHtml(result);
  }
}
