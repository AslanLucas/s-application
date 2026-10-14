import { expect, test } from '@playwright/test';

test('user can create and delete a note', async ({ page }) => {
  const title = `E2E ${Date.now()}`;
  await page.goto('/');

  await page.getByLabel('Titel').fill(title);
  await page.getByLabel('Inhalt').fill('Erstellt von Playwright');
  await page.getByRole('button', { name: 'Speichern' }).click();

  const item = page.getByRole('listitem').filter({ hasText: title });
  await expect(item).toBeVisible();

  await item.getByRole('button', { name: 'Löschen' }).click();
  await expect(item).toHaveCount(0);
});

test('created note is still there after reload', async ({ page }) => {
  const title = `Persist ${Date.now()}`;
  await page.goto('/');

  await page.getByLabel('Titel').fill(title);
  await page.getByLabel('Inhalt').fill('Muss in der Datenbank liegen');
  await page.getByRole('button', { name: 'Speichern' }).click();
  await expect(page.getByText(title)).toBeVisible();

  await page.reload();

  const item = page.getByRole('listitem').filter({ hasText: title });
  await expect(item).toBeVisible();

  await item.getByRole('button', { name: 'Löschen' }).click();
  await expect(item).toHaveCount(0);
});
