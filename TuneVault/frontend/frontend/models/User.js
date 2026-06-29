
class User {
  constructor(data) {
    this.id = data.id;
    this.displayName = data.displayName;
    this.email = data.email;
  }

  getInitials() {
    return this.displayName
      .split(' ')
      .map((p) => p[0])
      .join('')
      .slice(0, 2)
      .toUpperCase();
  }
}
