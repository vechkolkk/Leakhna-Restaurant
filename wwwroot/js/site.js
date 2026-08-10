document.querySelectorAll('[data-quantity-stepper]').forEach((stepper) => {
  const input = stepper.querySelector('input[type="number"]');

  stepper.querySelectorAll('[data-step]').forEach((button) => {
    button.addEventListener('click', () => {
      const step = Number(button.dataset.step);
      const min = Number(input.min || 0);
      const max = Number(input.max || 99);
      const nextValue = Math.min(max, Math.max(min, Number(input.value || 0) + step));
      input.value = nextValue;
    });
  });
});
